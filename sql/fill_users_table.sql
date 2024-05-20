-- паролі повинні бути хешовані через SHA256, наведені паролі - плейсхолдери
INSERT INTO users (username, password, is_admin) VALUES
('admin', 'hashedsha256_admin_password', TRUE),
('user', 'hashedsha256_user_password', FALSE);
