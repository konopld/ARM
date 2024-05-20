CREATE TABLE employees (
    id INT AUTO_INCREMENT PRIMARY KEY,
    surname VARCHAR(100) NOT NULL,
    department VARCHAR(100) NOT NULL,
    birth_year YEAR NOT NULL,
    employment_year YEAR NOT NULL,
    position VARCHAR(100) NOT NULL,
    academic_degree VARCHAR(100),
    academic_title VARCHAR(100)
);
