class AdminApp {
    constructor() {
        this.currentEntity = '';
        this.currentData = [];
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadInitialData();
        console.log('Admin app initialized');
    }

    bindEvents() {
        // Global search
        document.getElementById('globalSearch')?.addEventListener('input', this.debounce(this.handleGlobalSearch.bind(this), 300));

        // Modal handlers
        document.addEventListener('click', this.handleModalActions.bind(this));

        // Form submissions
        document.addEventListener('submit', this.handleFormSubmissions.bind(this));
    }

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    async handleGlobalSearch(e) {
        const term = e.target.value.trim();
        if (term.length < 2) return;

        try {
            const response = await fetch(`/Admin/Admin/Search?term=${encodeURIComponent(term)}`);
            const result = await response.json();

            if (result.success) {
                this.displaySearchResults(result.data);
            }
        } catch (error) {
            console.error('Search error:', error);
            this.showNotification('Search failed', 'error');
        }
    }

    displaySearchResults(data) {
        // Implement search results display
        console.log('Search results:', data);
    }

    async handleModalActions(e) {
        if (e.target.closest('.btn-add')) {
            e.preventDefault();
            await this.showAddModal();
        } else if (e.target.closest('.btn-edit')) {
            e.preventDefault();
            const id = e.target.closest('.btn-edit').dataset.id;
            await this.showEditModal(id);
        } else if (e.target.closest('.btn-delete')) {
            e.preventDefault();
            const id = e.target.closest('.btn-delete').dataset.id;
            await this.confirmDelete(id);
        } else if (e.target.closest('.btn-close') || e.target.closest('.btn-close-modal')) {
            this.hideModal();
        } else if (e.target.id === 'modalContainer' && e.target.classList.contains('modal')) {
            this.hideModal();
        }
    }

    handleFormSubmissions(e) {
        if (e.target.id === 'addUserForm' || e.target.id === 'editUserForm') {
            e.preventDefault();
            // Form submission is handled by specific methods
        }
    }

    async showAddModal(entityType = 'user') {
        try {
            let modalHtml = '';

            if (entityType === 'user') {
                modalHtml = this.getUserModalHtml(null, 'add');
            }
            // Add other entity types here

            document.getElementById('modalContainer').innerHTML = modalHtml;
        } catch (error) {
            console.error('Error showing add modal:', error);
            this.showNotification('Error opening form', 'error');
        }
    }

    async showEditModal(id, entityType = 'user') {
        try {
            let modalHtml = '';
            let data = null;

            if (entityType === 'user') {
                const response = await fetch(`/Admin/Admin/GetUser?id=${id}`);
                const result = await response.json();

                if (result.success) {
                    data = result.data;
                    modalHtml = this.getUserModalHtml(data, 'edit');
                } else {
                    throw new Error(result.message);
                }
            }
            // Add other entity types here

            document.getElementById('modalContainer').innerHTML = modalHtml;
        } catch (error) {
            console.error('Error showing edit modal:', error);
            this.showNotification('Error loading data', 'error');
        }
    }

    getUserModalHtml(user, action) {
        const isEdit = action === 'edit';
        const title = isEdit ? 'Edit User' : 'Add New User';
        const buttonText = isEdit ? 'Update User' : 'Create User';

        return `
            <div class="modal show">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3>${title}</h3>
                        <button class="btn-close">
                            <i class="bi bi-x"></i>
                        </button>
                    </div>
                    <form id="${isEdit ? 'editUserForm' : 'addUserForm'}">
                        <div class="modal-body">
                            ${isEdit ? `<input type="hidden" name="id" value="${user.id}">` : ''}
                            <div class="form-group">
                                <label class="form-label">Full Name *</label>
                                <input type="text" class="form-control" name="fullName" value="${isEdit ? user.fullName || '' : ''}" required>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Email *</label>
                                <input type="email" class="form-control" name="email" value="${isEdit ? user.email || '' : ''}" required>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Phone Number</label>
                                <input type="tel" class="form-control" name="phoneNumber" value="${isEdit ? user.phoneNumber || '' : ''}">
                            </div>
                            <div class="form-group">
                                <label class="form-label">Role *</label>
                                <select class="form-control form-select" name="role" required>
                                    <option value="">Select Role</option>
                                    <option value="Student" ${isEdit && user.role === 'Student' ? 'selected' : ''}>Student</option>
                                    <option value="Teacher" ${isEdit && user.role === 'Teacher' ? 'selected' : ''}>Teacher</option>
                                    <option value="Admin" ${isEdit && user.role === 'Admin' ? 'selected' : ''}>Admin</option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Address</label>
                                <textarea class="form-control form-textarea" name="address" rows="3">${isEdit ? user.address || '' : ''}</textarea>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-outline btn-close">Cancel</button>
                            <button type="submit" class="btn btn-primary">
                                ${isEdit ? 'Update User' : 'Create User'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        `;
    }

    hideModal() {
        document.getElementById('modalContainer').innerHTML = '';
    }

    async submitAddForm() {
        const form = document.getElementById('addUserForm');
        await this.submitUserForm(form, 'create');
    }

    async submitEditForm() {
        const form = document.getElementById('editUserForm');
        await this.submitUserForm(form, 'update');
    }

    async submitUserForm(form, action) {
        const formData = new FormData(form);
        const data = Object.fromEntries(formData);
        const url = action === 'create' ? '/Admin/Admin/CreateUser' : '/Admin/Admin/UpdateUser';

        try {
            // Show loading state
            const submitBtn = form.querySelector('button[type="submit"]');
            const originalText = submitBtn.innerHTML;
            submitBtn.innerHTML = '<div class="spinner"></div> Loading...';
            submitBtn.disabled = true;

            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();

            if (result.success) {
                this.showNotification(result.message, 'success');
                this.hideModal();
                // Refresh the page to show updated data
                setTimeout(() => location.reload(), 1000);
            } else {
                this.showNotification(result.message || 'Operation failed', 'error');
                // Show validation errors if any
                if (result.errors) {
                    console.error('Validation errors:', result.errors);
                }
            }
        } catch (error) {
            console.error(`Error ${action} user:`, error);
            this.showNotification(`Error ${action} user`, 'error');
        } finally {
            // Restore button state
            const submitBtn = form.querySelector('button[type="submit"]');
            submitBtn.innerHTML = action === 'create' ? 'Create User' : 'Update User';
            submitBtn.disabled = false;
        }
    }

    async confirmDelete(id, entityType = 'user') {
        if (!confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
            return;
        }

        try {
            let url = '';
            if (entityType === 'user') {
                url = `/Admin/Admin/DeleteUser?id=${id}`;
            }
            // Add other entity types here

            const response = await fetch(url, {
                method: 'POST'
            });

            const result = await response.json();

            if (result.success) {
                this.showNotification(result.message, 'success');
                // Refresh the page to show updated data
                setTimeout(() => location.reload(), 1000);
            } else {
                this.showNotification(result.message || 'Delete failed', 'error');
            }
        } catch (error) {
            console.error('Error deleting item:', error);
            this.showNotification('Error deleting item', 'error');
        }
    }

    showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.innerHTML = `
            <div style="display: flex; align-items: center; gap: 0.5rem;">
                <i class="bi bi-${this.getNotificationIcon(type)}"></i>
                <span>${message}</span>
            </div>
        `;

        document.getElementById('notificationContainer').appendChild(notification);

        // Remove after 5 seconds
        setTimeout(() => {
            notification.style.animation = 'slideInRight 0.3s ease reverse';
            setTimeout(() => notification.remove(), 300);
        }, 5000);
    }

    getNotificationIcon(type) {
        const icons = {
            success: 'check-circle',
            error: 'exclamation-circle',
            warning: 'exclamation-triangle',
            info: 'info-circle'
        };
        return icons[type] || 'info-circle';
    }

    loadInitialData() {
        // Load any initial data needed
        console.log('Loading initial data...');
    }

    // Utility methods
    formatDate(dateString) {
        return new Date(dateString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    }

    formatDateTime(dateString) {
        return new Date(dateString).toLocaleString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }
}

// Initialize the admin app when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.adminApp = new AdminApp();

    // Bind form submissions to adminApp methods
    document.addEventListener('submit', (e) => {
        if (e.target.id === 'addUserForm') {
            e.preventDefault();
            adminApp.submitAddForm();
        } else if (e.target.id === 'editUserForm') {
            e.preventDefault();
            adminApp.submitEditForm();
        }
    });
});

// Global functions for HTML onclick handlers
function toggleUserMenu() {
    console.log('User menu toggled');
    // Implement user menu functionality
}

function refreshDashboard() {
    if (window.adminApp) {
        adminApp.showNotification('Refreshing dashboard...', 'info');
        setTimeout(() => location.reload(), 500);
    }
}
// Trong class AdminApp, thêm các phương thức sau:

getProgramModalHtml(program, action) {
    const isEdit = action === 'edit';
    const title = isEdit ? 'Edit Program' : 'Add New Program';
    const buttonText = isEdit ? 'Update Program' : 'Create Program';

    return `
        <div class="modal show">
            <div class="modal-content">
                <div class="modal-header">
                    <h3>${title}</h3>
                    <button class="btn-close">
                        <i class="bi bi-x"></i>
                    </button>
                </div>
                <form id="${isEdit ? 'editProgramForm' : 'addProgramForm'}">
                    <div class="modal-body">
                        ${isEdit ? `<input type="hidden" name="id" value="${program.id}">` : ''}
                        <div class="form-group">
                            <label class="form-label">Program Name *</label>
                            <input type="text" class="form-control" name="name" value="${isEdit ? program.name || '' : ''}" required>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Description</label>
                            <textarea class="form-control form-textarea" name="description" rows="3">${isEdit ? program.description || '' : ''}</textarea>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Status</label>
                            <select class="form-control form-select" name="isActive">
                                <option value="true" ${isEdit && program.isActive ? 'selected' : ''}>Active</option>
                                <option value="false" ${isEdit && !program.isActive ? 'selected' : ''}>Inactive</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-outline btn-close">Cancel</button>
                        <button type="submit" class="btn btn-primary">
                            ${buttonText}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    `;
}

async submitProgramForm(form, action) {
    const formData = new FormData(form);
    const data = Object.fromEntries(formData);
    data.isActive = data.isActive === 'true';

    const url = action === 'create' ? '/Admin/Admin/CreateProgram' : '/Admin/Admin/UpdateProgram';

    try {
        const submitBtn = form.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;
        submitBtn.innerHTML = '<div class="spinner"></div> Loading...';
        submitBtn.disabled = true;

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.success) {
            this.showNotification(result.message, 'success');
            this.hideModal();
            setTimeout(() => location.reload(), 1000);
        } else {
            this.showNotification(result.message || 'Operation failed', 'error');
        }
    } catch (error) {
        console.error(`Error ${action} program:`, error);
        this.showNotification(`Error ${action} program`, 'error');
    } finally {
        const submitBtn = form.querySelector('button[type="submit"]');
        submitBtn.innerHTML = action === 'create' ? 'Create Program' : 'Update Program';
        submitBtn.disabled = false;
    }
}
init() {
    this.bindSearchEvents();
    console.log('Admin App Initialized');
}

bindSearchEvents() {
    const searchInput = document.getElementById('globalSearch');
    if (!searchInput) return;

    // Tạo dropdown container
    let dropdown = document.querySelector('.search-results-dropdown');
    if (!dropdown) {
        dropdown = document.createElement('div');
        dropdown.className = 'search-results-dropdown';
        searchInput.parentElement.appendChild(dropdown);
    }

    // Sự kiện gõ phím (Debounce 300ms)
    searchInput.addEventListener('input', this.debounce(async (e) => {
        const term = e.target.value.trim();
        if (term.length < 2) {
            dropdown.classList.remove('show');
            return;
        }

        try {
            const response = await fetch(`/Admin/Admin/Search?term=${encodeURIComponent(term)}`);
            const result = await response.json();

            if (result.success) {
                this.renderSearchResults(result.data, dropdown);
            }
        } catch (error) {
            console.error('Search error:', error);
        }
    }, 300));

    // Ẩn khi click ra ngoài
    document.addEventListener('click', (e) => {
        if (!searchInput.contains(e.target) && !dropdown.contains(e.target)) {
            dropdown.classList.remove('show');
        }
    });

    // Hiện lại khi focus
    searchInput.addEventListener('focus', () => {
        if (searchInput.value.trim().length >= 2 && dropdown.innerHTML !== '') {
            dropdown.classList.add('show');
        }
    });
}

renderSearchResults(data, container) {
    let html = '';
    const hasUsers = data.users && data.users.length > 0;
    const hasCourses = data.courses && data.courses.length > 0;
    const hasClasses = data.classes && data.classes.length > 0;

    if (!hasUsers && !hasCourses && !hasClasses) {
        container.innerHTML = '<div class="search-item"><div class="search-item-info"><p style="text-align:center; color:#64748b; padding:0.5rem;">No results found</p></div></div>';
        container.classList.add('show');
        return;
    }

    if (hasUsers) {
        html += `<div class="search-section-header">Users</div>`;
        data.users.forEach(u => {
            html += `<div class="search-item" onclick="window.location.href='/Admin/Admin/Users?search=${u.fullName}'">
                            <div class="search-item-icon"><i class="bi bi-person"></i></div>
                            <div class="search-item-info"><h4>${u.fullName}</h4><p>${u.email} • ${u.role || 'User'}</p></div>
                         </div>`;
        });
    }

    if (hasCourses) {
        html += `<div class="search-section-header">Courses</div>`;
        data.courses.forEach(c => {
            html += `<div class="search-item" onclick="window.location.href='/Admin/Admin/Course'">
                            <div class="search-item-icon"><i class="bi bi-book"></i></div>
                            <div class="search-item-info"><h4>${c.name}</h4><p>${c.instructor || 'No instructor'}</p></div>
                         </div>`;
        });
    }

    if (hasClasses) {
        html += `<div class="search-section-header">Classes</div>`;
        data.classes.forEach(c => {
            html += `<div class="search-item" onclick="window.location.href='/Admin/Admin/Class'">
                            <div class="search-item-icon"><i class="bi bi-door-open"></i></div>
                            <div class="search-item-info"><h4>${c.name}</h4><p>${c.room || 'No room'}</p></div>
                         </div>`;
        });
    }

    container.innerHTML = html;
    container.classList.add('show');
}

debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}
// --- 2. LOGIC MENU USER & LOGOUT (QUAN TRỌNG) ---

// Hàm bật/tắt menu khi click vào Avatar
function toggleUserMenu() {
    const dropdown = document.getElementById('userDropdown');
    const arrow = document.getElementById('userMenuArrow');
    const avatarBtn = document.querySelector('.user-avatar');

    if (dropdown) {
        dropdown.classList.toggle('show'); // Thêm/Xóa class .show để hiện/ẩn
        if (avatarBtn) avatarBtn.classList.toggle('active');

        // Xoay mũi tên
        if (arrow) {
            arrow.style.transform = dropdown.classList.contains('show') ? 'rotate(180deg)' : 'rotate(0deg)';
        }
    }
}

// Đóng menu khi click ra ngoài
window.addEventListener('click', function (e) {
    const userMenu = document.querySelector('.user-menu');
    // Nếu click KHÔNG nằm trong user-menu thì đóng lại
    if (userMenu && !userMenu.contains(e.target)) {
        const dropdown = document.getElementById('userDropdown');
        const arrow = document.getElementById('userMenuArrow');
        const avatarBtn = document.querySelector('.user-avatar');

        if (dropdown && dropdown.classList.contains('show')) {
            dropdown.classList.remove('show');
            if (avatarBtn) avatarBtn.classList.remove('active');
            if (arrow) arrow.style.transform = 'rotate(0deg)';
        }
    }
});

document.addEventListener('DOMContentLoaded', () => {
    window.adminApp = new AdminApp();
});
