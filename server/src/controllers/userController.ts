import { Request, Response } from 'express';
import bcrypt from 'bcryptjs';
import sql from 'mssql';
import { executeStoredProcedure, useLocalJsonDb, readJsonUsers, writeJsonUsers } from '../db';

export async function registerUser(req: Request, res: Response): Promise<void> {
    try {
        const { username, password, fullName, designation } = req.body;

        // Mandatory Validations
        if (!username || typeof username !== 'string' || username.trim() === '') {
            res.status(400).json({ success: false, message: 'Username is required.' });
            return;
        }
        if (!password || typeof password !== 'string' || password.trim() === '') {
            res.status(400).json({ success: false, message: 'Password is required.' });
            return;
        }
        if (!fullName || typeof fullName !== 'string' || fullName.trim() === '') {
            res.status(400).json({ success: false, message: 'Full Name is required.' });
            return;
        }
        if (!designation || typeof designation !== 'string' || designation.trim() === '') {
            res.status(400).json({ success: false, message: 'Designation is required.' });
            return;
        }

        // Password Length
        if (password.length < 8) {
            res.status(400).json({ success: false, message: 'Password must be at least 8 characters long.' });
            return;
        }

        // Username Format
        const usernameRegex = /^[a-zA-Z0-9_.-]+$/;
        if (!usernameRegex.test(username.trim())) {
            res.status(400).json({ 
                success: false, 
                message: 'Username can only contain alphanumeric characters, underscores, hyphens, and periods.' 
            });
            return;
        }

        const salt = await bcrypt.genSalt(10);
        const hashedPassword = await bcrypt.hash(password, salt);

        // JSON Fallback
        if (useLocalJsonDb) {
            const users = readJsonUsers();
            const usernameLower = username.trim().toLowerCase();
            if (users.some(u => u.username.toLowerCase() === usernameLower)) {
                res.status(409).json({ success: false, message: 'Username is already taken.' });
                return;
            }

            const newUserId = users.length > 0 ? Math.max(...users.map(u => u.userId)) + 1 : 1001;
            const newUser = {
                userId: newUserId,
                username: username.trim(),
                passwordHash: hashedPassword,
                fullName: fullName.trim(),
                designation: designation.trim(),
                status: 'Active',
                createdDate: new Date().toISOString()
            };

            users.push(newUser);
            writeJsonUsers(users);

            res.status(201).json({
                success: true,
                message: 'Registration successful.',
                data: {
                    userId: newUser.userId,
                    username: newUser.username,
                    fullName: newUser.fullName,
                    designation: newUser.designation,
                    status: newUser.status
                }
            });
            return;
        }

        // SQL Server Execution
        const inputs = [
            { name: 'Username', type: sql.NVarChar(50), value: username.trim() },
            { name: 'PasswordHash', type: sql.NVarChar(255), value: hashedPassword },
            { name: 'FullName', type: sql.NVarChar(150), value: fullName.trim() },
            { name: 'Designation', type: sql.NVarChar(100), value: designation.trim() },
            { name: 'Status', type: sql.NVarChar(20), value: 'Active' }
        ];
        const outputs = [
            { name: 'NewUserId', type: sql.Int }
        ];

        const result = await executeStoredProcedure<any>('dbo.usp_User_Register', inputs, outputs);
        const newUserId = result.output.NewUserId;

        res.status(201).json({
            success: true,
            message: 'Registration successful.',
            data: {
                userId: newUserId,
                username: username.trim(),
                fullName: fullName.trim(),
                designation: designation.trim(),
                status: 'Active'
            }
        });
    } catch (error) {
        const err = error as Error;
        if (err.message.includes('Conflict Error') || err.message.includes('Violation of UNIQUE KEY constraint')) {
            res.status(409).json({ success: false, message: 'Username is already taken.' });
            return;
        }
        res.status(500).json({ success: false, message: 'An internal server error occurred.' });
    }
}

export async function loginUser(req: Request, res: Response): Promise<void> {
    try {
        const { username, password } = req.body;

        if (!username || typeof username !== 'string' || username.trim() === '') {
            res.status(400).json({ success: false, message: 'Username is required.' });
            return;
        }
        if (!password || typeof password !== 'string' || password.trim() === '') {
            res.status(400).json({ success: false, message: 'Password is required.' });
            return;
        }

        let user: any = null;

        if (useLocalJsonDb) {
            const users = readJsonUsers();
            const usernameLower = username.trim().toLowerCase();
            user = users.find(u => u.username.toLowerCase() === usernameLower);
        } else {
            const inputs = [
                { name: 'Username', type: sql.NVarChar(50), value: username.trim() }
            ];
            const result = await executeStoredProcedure<any>('dbo.usp_User_GetByUsername', inputs);
            if (result.recordset && result.recordset.length > 0) {
                user = result.recordset[0];
            }
        }

        if (!user) {
            res.status(401).json({ success: false, message: 'Invalid username or password.' });
            return;
        }

        const userStatus = user.status || user.Status;
        if (userStatus !== 'Active') {
            res.status(403).json({ success: false, message: 'Your account is inactive. Please contact your administrator.' });
            return;
        }

        const storedHash = user.passwordHash || user.PasswordHash;
        const isPasswordMatch = await bcrypt.compare(password, storedHash);
        if (!isPasswordMatch) {
            res.status(401).json({ success: false, message: 'Invalid username or password.' });
            return;
        }

        res.status(200).json({
            success: true,
            message: 'Login successful.',
            data: {
                userId: user.userId || user.UserId,
                username: user.username || user.Username,
                fullName: user.fullName || user.FullName,
                designation: user.designation || user.Designation,
                status: userStatus,
                createdDate: user.createdDate || user.CreatedDate
            }
        });
    } catch (error) {
        res.status(500).json({ success: false, message: 'An internal server error occurred.' });
    }
}
