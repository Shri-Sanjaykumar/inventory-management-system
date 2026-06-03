import express from 'express';
import cors from 'cors';
import path from 'path';
import { getDbPool } from './db';
import { registerUser, loginUser } from './controllers/userController';

const app = express();
const PORT = 5000;

app.use(cors());
app.use(express.json());
app.use(express.static(path.join(__dirname, '../../public')));

app.post('/api/users/register', registerUser);
app.post('/api/users/login', loginUser);

app.get('/api/health', async (req, res) => {
    try {
        const pool = await getDbPool();
        if (pool && pool.connected) {
            res.status(200).json({ status: 'UP', database: 'CONNECTED' });
        } else {
            res.status(200).json({ status: 'UP', database: 'JSON_FALLBACK' });
        }
    } catch (error) {
        res.status(500).json({ status: 'DOWN', error: (error as Error).message });
    }
});

app.get('*', (req, res) => {
    res.sendFile(path.join(__dirname, '../../public/index.html'));
});

async function startServer() {
    try {
        await getDbPool();
        app.listen(PORT, () => {
            console.log(`[Server] Application running successfully on port ${PORT}`);
            console.log(`[Server] Local URL: http://localhost:${PORT}`);
        });
    } catch (error) {
        console.error('[Server] Critical DB initialization error:', (error as Error).message);
    }
}

startServer();
