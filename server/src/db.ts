import sql from 'mssql';
import fs from 'fs';
import path from 'path';

export let useLocalJsonDb = false;
const JSON_DB_PATH = path.join(__dirname, '../../users.json');

// Hardcoded database configuration to make setup zero-config
const dbConfig: sql.config = {
    user: 'sa',
    password: 'Password_123!',
    server: 'localhost',
    instanceName: 'SQLEXPRESS',
    database: 'UserManagementDB',
    options: {
        encrypt: false,
        trustServerCertificate: true,
        enableArithAbort: true
    },
    pool: {
        max: 5,
        min: 0,
        idleTimeoutMillis: 15000
    }
};

let pool: sql.ConnectionPool | null = null;

export async function getDbPool(): Promise<sql.ConnectionPool | null> {
    if (useLocalJsonDb) return null;
    if (pool && pool.connected) return pool;

    const maxRetries = 2;
    const retryIntervalMs = 2000;

    for (let attempt = 1; attempt <= maxRetries; attempt++) {
        try {
            console.log(`[Database] Connecting to SQL Server at localhost\\SQLEXPRESS... (Attempt ${attempt}/${maxRetries})`);
            pool = await new sql.ConnectionPool(dbConfig).connect();
            console.log(`[Database] Connected successfully to SQL Server: ${dbConfig.database}`);
            useLocalJsonDb = false;
            return pool;
        } catch (error) {
            console.warn(`[Database] Connection attempt ${attempt} failed:`, (error as Error).message);
            
            if (attempt === maxRetries) {
                console.warn('\n[Database] SQL Server (localhost\\SQLEXPRESS) is not reachable.');
                console.log(`[Database] Falling back to local JSON database: ${JSON_DB_PATH}`);
                console.log('[Database] If SQL Server is running, restart the service in Windows Services to enable sa authentication.');
                useLocalJsonDb = true;
                initializeJsonDb();
                return null;
            }
            await new Promise(resolve => setTimeout(resolve, retryIntervalMs));
        }
    }
    return null;
}

function initializeJsonDb() {
    if (!fs.existsSync(JSON_DB_PATH)) {
        fs.writeFileSync(JSON_DB_PATH, JSON.stringify([], null, 2), 'utf-8');
        console.log('[Database] Created new local database file: users.json');
    }
}

export function readJsonUsers(): any[] {
    initializeJsonDb();
    try {
        const data = fs.readFileSync(JSON_DB_PATH, 'utf-8');
        return JSON.parse(data);
    } catch (e) {
        console.error('[Database] Error reading JSON DB:', e);
        return [];
    }
}

export function writeJsonUsers(users: any[]): void {
    try {
        fs.writeFileSync(JSON_DB_PATH, JSON.stringify(users, null, 2), 'utf-8');
    } catch (e) {
        console.error('[Database] Error writing to JSON DB:', e);
    }
}

export async function executeStoredProcedure<T>(
    procName: string,
    inputs: { name: string; type: sql.ISqlType; value: any }[] = [],
    outputs: { name: string; type: sql.ISqlType }[] = []
): Promise<{ recordset: T[]; output: { [key: string]: any }; returnValue: any }> {
    const dbPool = await getDbPool();
    if (!dbPool) {
        throw new Error('Database pool not available (operating in JSON fallback mode).');
    }

    const request = dbPool.request();
    inputs.forEach(input => request.input(input.name, input.type, input.value));
    outputs.forEach(output => request.output(output.name, output.type));

    const result = await request.execute(procName);
    return {
        recordset: result.recordset as T[],
        output: result.output,
        returnValue: result.returnValue
    };
}
