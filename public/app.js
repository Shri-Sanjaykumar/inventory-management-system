const API_BASE = '/api';

const authCard = document.getElementById('authCard');
const dashboardCard = document.getElementById('dashboardCard');
const loginForm = document.getElementById('loginForm');
const registerForm = document.getElementById('registerForm');
const tabLogin = document.getElementById('tabLogin');
const tabRegister = document.getElementById('tabRegister');
const cardSubtitle = document.getElementById('cardSubtitle');

document.addEventListener('DOMContentLoaded', () => {
    const session = localStorage.getItem('user_session');
    if (session) {
        try {
            showDashboard(JSON.parse(session));
        } catch (e) {
            localStorage.removeItem('user_session');
            showAuthCard();
        }
    } else {
        showAuthCard();
    }
});

function togglePassword(inputId) {
    const input = document.getElementById(inputId);
    const icon = input.parentElement.querySelector('.toggle-password');
    if (input.type === 'password') {
        input.type = 'text';
        icon.className = 'fa-regular fa-eye toggle-password';
    } else {
        input.type = 'password';
        icon.className = 'fa-regular fa-eye-slash toggle-password';
    }
}

function switchTab(mode) {
    clearErrors();
    if (mode === 'login') {
        tabLogin.classList.add('active');
        tabRegister.classList.remove('active');
        loginForm.classList.add('active');
        registerForm.classList.remove('active');
        cardSubtitle.textContent = 'Please sign in to access your dashboard';
    } else {
        tabLogin.classList.remove('active');
        tabRegister.classList.add('active');
        loginForm.classList.remove('active');
        registerForm.classList.add('active');
        cardSubtitle.textContent = 'Create a new user profile';
    }
}

function clearErrors() {
    document.querySelectorAll('.error-msg').forEach(el => el.textContent = '');
    document.querySelectorAll('input').forEach(input => input.style.borderColor = '');
}

function setError(id, msg) {
    const input = document.getElementById(id);
    const errorEl = document.getElementById(`err-${id}`);
    if (input) input.style.borderColor = 'var(--error-color)';
    if (errorEl) errorEl.textContent = msg;
}

function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    
    const iconClass = type === 'success' ? 'fa-regular fa-circle-check' : 'fa-regular fa-circle-xmark';
    toast.innerHTML = `<i class="${iconClass}"></i><span>${message}</span>`;
    
    container.appendChild(toast);

    setTimeout(() => {
        toast.classList.add('fade-out');
        setTimeout(() => toast.remove(), 250);
    }, 3500);
}

async function handleLogin(event) {
    event.preventDefault();
    clearErrors();

    const usernameInput = document.getElementById('loginUsername');
    const passwordInput = document.getElementById('loginPassword');
    const submitBtn = document.getElementById('loginSubmitBtn');

    const username = usernameInput.value.trim();
    const password = passwordInput.value;

    let valid = true;
    if (!username) {
        setError('loginUsername', 'Username is required.');
        valid = false;
    }
    if (!password) {
        setError('loginPassword', 'Password is required.');
        valid = false;
    }

    if (!valid) return;

    setLoading(submitBtn, true, 'Signing In...');

    try {
        const res = await fetch(`${API_BASE}/users/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });
        const result = await res.json();

        if (res.ok && result.success) {
            showToast('Signed in successfully.', 'success');
            localStorage.setItem('user_session', JSON.stringify(result.data));
            setTimeout(() => {
                showDashboard(result.data);
                loginForm.reset();
            }, 600);
        } else {
            showToast(result.message || 'Login failed.', 'error');
        }
    } catch (e) {
        showToast('Server connection failed.', 'error');
    } finally {
        setLoading(submitBtn, false, 'Sign In');
    }
}

async function handleRegister(event) {
    event.preventDefault();
    clearErrors();

    const fullNameInput = document.getElementById('regFullName');
    const designationInput = document.getElementById('regDesignation');
    const usernameInput = document.getElementById('regUsername');
    const passwordInput = document.getElementById('regPassword');
    const submitBtn = document.getElementById('registerSubmitBtn');

    const fullName = fullNameInput.value.trim();
    const designation = designationInput.value.trim();
    const username = usernameInput.value.trim();
    const password = passwordInput.value;

    let valid = true;
    if (!fullName) { setError('regFullName', 'Full Name is required.'); valid = false; }
    if (!designation) { setError('regDesignation', 'Designation is required.'); valid = false; }
    if (!username) { setError('regUsername', 'Username is required.'); valid = false; }
    if (!password) { setError('regPassword', 'Password is required.'); valid = false; }

    if (password && password.length < 8) {
        setError('regPassword', 'Password must be at least 8 characters.');
        valid = false;
    }

    const usernameRegex = /^[a-zA-Z0-9_.-]+$/;
    if (username && !usernameRegex.test(username)) {
        setError('regUsername', 'Invalid characters (use a-z, 0-9, _, -, .).');
        valid = false;
    }

    if (!valid) return;

    setLoading(submitBtn, true, 'Registering...');

    try {
        const res = await fetch(`${API_BASE}/users/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName, designation, username, password })
        });
        const result = await res.json();

        if (res.ok && result.success) {
            showToast('Registration successful.', 'success');
            registerForm.reset();
            setTimeout(() => {
                switchTab('login');
                document.getElementById('loginUsername').value = username;
            }, 1000);
        } else {
            showToast(result.message || 'Registration failed.', 'error');
            if (res.status === 409) {
                setError('regUsername', 'Username is already taken.');
            }
        }
    } catch (e) {
        showToast('Server connection failed.', 'error');
    } finally {
        setLoading(submitBtn, false, 'Register Account');
    }
}

function handleLogout() {
    localStorage.removeItem('user_session');
    showToast('Signed out successfully.', 'success');
    dashboardCard.classList.remove('active');
    setTimeout(() => {
        dashboardCard.style.display = 'none';
        showAuthCard();
    }, 200);
}

function setLoading(button, isLoading, text) {
    button.disabled = isLoading;
    const label = button.querySelector('span');
    if (label) label.textContent = text;
}

function showAuthCard() {
    dashboardCard.style.display = 'none';
    dashboardCard.classList.remove('active');
    authCard.style.display = 'block';
    setTimeout(() => authCard.classList.add('active'), 50);
}

function showDashboard(user) {
    authCard.classList.remove('active');
    setTimeout(() => {
        authCard.style.display = 'none';
        
        document.getElementById('dashFullName').textContent = user.fullName;
        document.getElementById('dashUserId').textContent = user.userId;
        document.getElementById('dashUsername').textContent = user.username;
        document.getElementById('dashDesignation').textContent = user.designation;
        document.getElementById('dashStatus').textContent = user.status;
        
        if (user.createdDate) {
            const date = new Date(user.createdDate);
            document.getElementById('dashCreatedDate').textContent = date.toLocaleDateString(undefined, {
                year: 'numeric', month: 'long', day: 'numeric'
            });
        }

        const nameParts = user.fullName.split(' ');
        const initials = nameParts.length > 1 
            ? (nameParts[0][0] + nameParts[1][0]).toUpperCase()
            : nameParts[0].substring(0, 2).toUpperCase();
        document.getElementById('userAvatar').textContent = initials;

        dashboardCard.style.display = 'block';
        setTimeout(() => dashboardCard.classList.add('active'), 50);
    }, 200);
}
