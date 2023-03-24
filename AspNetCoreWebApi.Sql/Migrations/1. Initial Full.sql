CREATE TABLE students
(
    -- Define a primary key (PK) for this table.
    -- TEXT length must be validated from the app.
    student_id TEXT
        -- Set the PK constraint.
        CONSTRAINT students_pkey PRIMARY KEY,
    
    -- Phone number data is mandatory.
    phone_number TEXT NOT NULL,
    
    full_name TEXT NOT NULL,
    
    -- Nickname is not mandatory.
    nickname TEXT NULL,
    
    -- Store the joined at date in date time with timezone format.
    joined_at TIMESTAMPTZ NOT NULL
);

CREATE TABLE schools
(
    school_id INT GENERATED ALWAYS AS IDENTITY
        CONSTRAINT school_pkey PRIMARY KEY,
    
    -- Using double-quote to escape the reserved keyword in PG, such as "name".
    "name" TEXT NOT NULL,
    
    established_at TIMESTAMPTZ NOT NULL
);

ALTER TABLE students
ADD COLUMN school_id INT NULL
    CONSTRAINT students__school_fkey REFERENCES schools;

CREATE TABLE lecturers
(
	lecturer_id INT GENERATED ALWAYS AS IDENTITY
		CONSTRAINT lecturers_pkey PRIMARY KEY,
	full_name TEXT NOT NULL,
	subject TEXT NOT NULL,
	school_id INT
		CONSTRAINT lecturers__schools_fkey REFERENCES schools	
);

CREATE TABLE learning_classes
(
	learning_class_id TEXT
		CONSTRAINT learning_classes_pkey PRIMARY KEY,
	start_date TIMESTAMPTZ NOT NULL,
	finish_date TIMESTAMPTZ NOT NULL,
	lecturer_id INT
		CONSTRAINT learning_classes__lecturers_fkey REFERENCES lecturers
);

CREATE TABLE learning_class_students
(
	learning_class_id TEXT,
	student_id TEXT,
	CONSTRAINT learning_class_students_pkey PRIMARY KEY(learning_class_id, student_id)
);