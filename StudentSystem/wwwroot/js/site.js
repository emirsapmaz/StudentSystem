// Authentication Scripts
    document.addEventListener('DOMContentLoaded', function () {
        // Get all alerts with class 'alert-dismissible'
        const alerts = document.querySelectorAll('.alert-dismissible');

        alerts.forEach(alert => {
        // Set timeout to close alert after 5 seconds
        setTimeout(() => {
            // Use Bootstrap's alert API to close it
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        }, 3000); 
        });
    });
document.addEventListener('DOMContentLoaded', function () {
    // Add fade-in animation to form container
    const formContainer = document.querySelector('.auth-form-container');
    if (formContainer) {
        formContainer.classList.add('fade-in');
    }

    // Add slide-in animation to brand content
    const brandContent = document.querySelector('.brand-content');
    if (brandContent) {
        brandContent.classList.add('slide-in-right');
    }
    // Form validation
    const forms = document.querySelectorAll('.auth-form');
    forms.forEach(form => {
    /*
        form.addEventListener('submit', handleFormSubmit);
*/

        // Real-time validation
        const inputs = form.querySelectorAll('input, select');
        inputs.forEach(input => {
            input.addEventListener('blur', validateField);
            input.addEventListener('input', clearValidation);
        });
    });
    // Password confirmation validation
    const confirmPasswordField = document.getElementById('confirmPassword');
    if (confirmPasswordField) {
        confirmPasswordField.addEventListener('input', validatePasswordConfirmation);
    }

    // Initialize tooltips if Bootstrap is available
    if (typeof bootstrap !== 'undefined') {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});

// Toggle password visibility
function togglePassword(fieldId) {
    const field = document.getElementById(fieldId);
    const toggle = field.nextElementSibling.querySelector('.password-toggle i');

    if (field.type === 'password') {
        field.type = 'text';
        toggle.classList.remove('fa-eye');
        toggle.classList.add('fa-eye-slash');
    } else {
        field.type = 'password';
        toggle.classList.remove('fa-eye-slash');
        toggle.classList.add('fa-eye');
    }
}
/*
// Toggle teacher subject field based on role selection
function toggleTeacherSubject() {
    const roleSelect = document.getElementById('role');
    const teacherSubjectGroup = document.getElementById('teacherSubjectGroup');
    const teacherSubjectField = document.getElementById('teacherSubject');
    
    if (roleSelect && teacherSubjectGroup) {
        if (roleSelect.value === 'Teacher') {
            teacherSubjectGroup.style.display = 'block';
            teacherSubjectField.required = true;
            // Add animation
            teacherSubjectGroup.classList.add('fade-in');
        } else {
            teacherSubjectGroup.style.display = 'none';
            teacherSubjectField.required = false;
            teacherSubjectField.value = '';
            teacherSubjectGroup.classList.remove('fade-in');
        }
    }
}

// Form submission handler
function handleFormSubmit(event) {
    event.preventDefault();
    
    const form = event.target;
    const submitButton = form.querySelector('.auth-btn');
    const isValid = validateForm(form);
    
    if (!isValid) {
        return false;
    }
    
    // Add loading state
    submitButton.classList.add('loading');
    submitButton.disabled = true;
    
    // Simulate form submission (replace with actual submission logic)
    setTimeout(() => {
        // Remove loading state
        submitButton.classList.remove('loading');
        submitButton.disabled = false;
        
        // In a real application, you would submit the form here
        // form.submit();
        
        // For demo purposes, show success message
        showNotification('Form submitted successfully!', 'success');
    }, 2000);
}
*/
// Validate entire form
function validateForm(form) {
    let isValid = true;
    const inputs = form.querySelectorAll('input[required], select[required]');

    inputs.forEach(input => {
        if (!validateField({ target: input })) {
            isValid = false;
        }
    });

    // Additional validation for password confirmation
    const password = form.querySelector('#password');
    const confirmPassword = form.querySelector('#confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
        showFieldError(confirmPassword, 'Passwords do not match');
        isValid = false;
    }

    return isValid;
}

// Validate individual field
function validateField(event) {
    const field = event.target;
    const value = field.value.trim();
    let isValid = true;

    // Clear previous validation
    clearValidation(event);

    // Required field validation
    if (field.hasAttribute('required') && !value) {
        showFieldError(field, 'This field is required');
        isValid = false;
    }

    // Email validation
    if (field.type === 'email' && value) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value)) {
            showFieldError(field, 'Please enter a valid email address');
            isValid = false;
        }
    }

    // Password validation
    if (field.type === 'password' && field.id === 'password' && value) {
        if (value.length < 6) {
            showFieldError(field, 'Password must be at least 6 characters long');
            isValid = false;
        }
    }

    // Show success state for valid fields
    if (isValid && value) {
        showFieldSuccess(field);
    }

    return isValid;
}

// Validate password confirmation
function validatePasswordConfirmation(event) {
    const confirmPassword = event.target;
    const password = document.getElementById('password');

    clearValidation(event);

    if (confirmPassword.value && password.value !== confirmPassword.value) {
        showFieldError(confirmPassword, 'Passwords do not match');
        return false;
    } else if (confirmPassword.value && password.value === confirmPassword.value) {
        showFieldSuccess(confirmPassword);
        return true;
    }

    return true;
}

// Clear field validation
function clearValidation(event) {
    const field = event.target;
    field.classList.remove('is-invalid', 'is-valid');

    // Remove existing feedback
    const existingFeedback = field.parentNode.querySelector('.invalid-feedback, .valid-feedback');
    if (existingFeedback) {
        existingFeedback.remove();
    }
}

// Show field error
function showFieldError(field, message) {
    field.classList.add('is-invalid');
    field.classList.remove('is-valid');

    // Create error message
    const feedback = document.createElement('div');
    feedback.className = 'invalid-feedback';
    feedback.textContent = message;

    // Insert after the field
    field.parentNode.appendChild(feedback);
}

// Show field success
function showFieldSuccess(field) {
    field.classList.add('is-valid');
    field.classList.remove('is-invalid');
}

function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    // Add to page
    document.body.appendChild(notification);

    // Auto dismiss after 5 seconds with fade-out
    setTimeout(() => {
        let bsAlert = bootstrap.Alert.getOrCreateInstance(notification);
        bsAlert.close();
    }, 5000);
}


// Smooth scroll for anchor links
document.addEventListener('click', function (event) {
    if (event.target.matches('a[href^="#"]')) {
        event.preventDefault();
        const target = document.querySelector(event.target.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    }
});

// Add keyboard navigation support
document.addEventListener('keydown', function (event) {
    // Enter key on buttons
    if (event.key === 'Enter' && event.target.matches('button:not([type="submit"])')) {
        event.target.click();
    }

    // Escape key to close modals or clear focus
    if (event.key === 'Escape') {
        const activeElement = document.activeElement;
        if (activeElement && activeElement.blur) {
            activeElement.blur();
        }
    }
});

// Form auto-save (optional feature)
function enableAutoSave(formId) {
    const form = document.getElementById(formId);
    if (!form) return;

    const inputs = form.querySelectorAll('input, select, textarea');

    inputs.forEach(input => {
        // Load saved value
        const savedValue = localStorage.getItem(`autosave_${input.name}`);
        if (savedValue && input.type !== 'password') {
            input.value = savedValue;
        }

        // Save on change
        input.addEventListener('input', function () {
            if (input.type !== 'password') {
                localStorage.setItem(`autosave_${input.name}`, input.value);
            }
        });
    });

    // Clear auto-save on successful submission
    form.addEventListener('submit', function () {
        inputs.forEach(input => {
            localStorage.removeItem(`autosave_${input.name}`);
        });
    });
}

// Initialize auto-save for forms (uncomment if needed)
// enableAutoSave('loginForm');
// enableAutoSave('registerForm');