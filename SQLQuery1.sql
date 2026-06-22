-- ============================================================
--  SEED DATA SCRIPT  -  Pet Grooming Shop
--  Run this against your database after deploying the schema.
-- ============================================================

SET NOCOUNT ON;

-- ============================================================
-- 0. CLEANUP EXISTING DATA (Prevents Duplicate & FK Errors)
--    Deletes data in reverse dependency order and resets IDs to 1
-- ============================================================
PRINT 'Cleaning up existing data...';

DELETE FROM [dbo].[InvoiceItems];
DELETE FROM [dbo].[Invoices];
DELETE FROM [dbo].[WalkInQueue];
DELETE FROM [dbo].[Appointments];
DELETE FROM [dbo].[Services];
DELETE FROM [dbo].[Pets];
DELETE FROM [dbo].[Customers];
DELETE FROM [dbo].[Staff];

DBCC CHECKIDENT ('[dbo].[Staff]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Customers]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Pets]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Services]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Appointments]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[WalkInQueue]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Invoices]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[InvoiceItems]', RESEED, 0);

PRINT 'Cleanup complete. Inserting fresh seed data...';

-- ============================================================
-- 1. STAFF
--    Roles allowed: 'Receptionist' | 'Groomer' | 'Admin'
-- ============================================================
INSERT INTO [dbo].[Staff] (FullName, Username, Email, Phone, PasswordHash, Role, IsActive)
VALUES
  ('Ahmad Fauzi',      'ahmad.fauzi',    'ahmad.fauzi@petshop.my',    '0123456781', 'AE2D5E1F3C9B7A4D6E8F2A0C5B3D1E7F9A2C4B6D8E0F1A3C5B7D9E2F4A6C8B0', 'Admin', 1),
  ('Nurul Izzah',      'nurul.izzah',    'nurul.izzah@petshop.my',    '0123456782', 'BF3E6F2G4D0A8B5E7F9A1C3B5D7E9F1A3C5B7D9E1F3A5C7B9D1E3F5A7C9B1D3', 'Receptionist', 1),
  ('Hafiz Ramli',      'hafiz.ramli',    'hafiz.ramli@petshop.my',    '0123456783', 'CF4F7G3H5E1B9C6F8G0B2D4C6E8F0B2D4C6E8F0B2D4C6E8F0B2D4C6E8F0B2D4C', 'Groomer', 1),
  ('Siti Rahayu',      'siti.rahayu',    'siti.rahayu@petshop.my',    '0123456784', 'DG5G8H4I6F2C0D7G9H1C3E5D7F9G1C3E5D7F9G1C3E5D7F9G1C3E5D7F9G1C3E5D', 'Groomer', 1),
  ('Farah Liyana',     'farah.liyana',   'farah.liyana@petshop.my',   '0123456785', 'EH6H9I5J7G3D1E8H0I2D4F6E8G0H2D4F6E8G0H2D4F6E8G0H2D4F6E8G0H2D4F6E', 'Groomer', 1),
  ('Zulaikha Mazlan',  'zulaikha.m',     'zulaikha.m@petshop.my',     '0123456786', 'FI7I0J6K8H4E2F9I1J3E5G7F9H1I3E5G7F9H1I3E5G7F9H1I3E5G7F9H1I3E5G7F', 'Receptionist', 1);

-- ============================================================
-- 2. CUSTOMERS
-- ============================================================
INSERT INTO [dbo].[Customers] (FullName, Phone, Email, Address, IsActive)
VALUES
  ('Ali Hassan',          '0111234567', 'ali.hassan@gmail.com',       'No 12, Jalan Melati, Melaka',            1),
  ('Salmah Othman',       '0119876543', 'salmah.othman@yahoo.com',    'No 5, Lorong Mawar, Ayer Keroh',         1),
  ('Tan Wei Ming',        '0125551234', 'tanweiming@hotmail.com',     'Blok B-3-5, Taman Permai, Melaka',      1),
  ('Priya Subramaniam',   '0137778888', 'priya.sub@gmail.com',        'No 88, Jalan Bunga, Alor Gajah',         1),
  ('Mohd Rashid Ismail',  '0163334444', 'rashid.ismail@gmail.com',    'No 22, Jalan Cempaka, Bemban',           1),
  ('Lee Siew Ling',       '0175556666', 'leesiewling@gmail.com',      'No 3, Taman Ixora, Bukit Baru',          1),
  ('Rohani Abdullah',     '0182227777', 'rohani.abd@yahoo.com',       'No 45, Jalan Seroja, Jasin',             1),
  ('Kumar Selvam',        '0191118888', 'kumar.selvam@gmail.com',     'No 7, Jalan Anggerik, Merlimau',         1),
  ('Noraini Hamid',       '0121239876', 'noraini.h@gmail.com',        'No 16, Taman Mutiara, Ayer Keroh',      0),
  ('Chong Boon Keat',     '0134567890', 'chong.bk@gmail.com',         'No 9, Jalan Kenanga, Klebang',           1);

-- ============================================================
-- 3. PETS (CustomerID 1–10)
-- ============================================================
INSERT INTO [dbo].[Pets] (CustomerID, Name, Species, Breed, Gender, DateOfBirth, Color, Weight, Allergies, MedicalNotes, IsActive)
VALUES
  (1, 'Comel',    'Dog',  'Golden Retriever',  'Male',   '2021-03-15', 'Golden',     28.50, NULL,            NULL,                        1),
  (1, 'Putih',    'Cat',  'Persian',           'Female', '2022-07-20', 'White',       4.20, 'Dust',          'Indoor only',               1),
  (2, 'Kopi',     'Dog',  'Poodle',            'Male',   '2020-11-05', 'Brown',       6.80, NULL,            NULL,                        1),
  (3, 'Biscuit',  'Cat',  'British Shorthair', 'Female', '2019-05-10', 'Grey',        5.50, NULL,            'Neutered',                  1),
  (3, 'Pepper',   'Dog',  'Shih Tzu',          'Male',   '2021-08-22', 'Black/White', 5.10, NULL,            NULL,                        1),
  (4, 'Raja',     'Dog',  'German Shepherd',   'Male',   '2018-12-01', 'Black/Tan',  32.00, NULL,            'Hip dysplasia, gentle care',1),
  (5, 'Manja',    'Cat',  'Domestic Shorthair','Female', '2023-01-30', 'Orange',      3.80, 'Chicken',       NULL,                        1),
  (6, 'Snow',     'Dog',  'Samoyed',           'Female', '2020-06-14', 'White',      22.00, NULL,            NULL,                        1),
  (6, 'Cloud',    'Cat',  'Ragdoll',           'Male',   '2021-09-09', 'Cream/Grey',  6.00, NULL,            NULL,                        1),
  (7, 'Harimau',  'Cat',  'Maine Coon',        'Male',   '2019-04-25', 'Brown Tabby', 8.50, NULL,            NULL,                        1),
  (8, 'Rocky',    'Dog',  'Labrador',          'Male',   '2020-02-18', 'Black',      30.00, NULL,            NULL,                        1),
  (9, 'Bunga',    'Cat',  'Siamese',           'Female', '2022-03-03', 'Seal Point',  4.00, NULL,            NULL,                        1),
  (10,'Oyen',     'Cat',  'Domestic Shorthair','Male',   '2021-12-12', 'Orange',      4.50, NULL,            NULL,                        1),
  (10,'Dino',     'Dog',  'Beagle',            'Male',   '2022-05-07', 'Tricolor',    12.00, 'Peanuts',       NULL,                        1);

-- ============================================================
-- 4. SERVICES
-- ============================================================
INSERT INTO [dbo].[Services] (ServiceName, Category, Price, DurationMins, IsActive)
VALUES
  ('Basic Bath & Dry',          'Grooming',  35.00,  60,  1),
  ('Full Grooming (Small)',      'Grooming',  65.00,  90,  1),
  ('Full Grooming (Medium)',     'Grooming',  85.00, 120,  1),
  ('Full Grooming (Large)',      'Grooming', 110.00, 150,  1),
  ('Nail Trimming',              'Add-On',    15.00,  20,  1),
  ('Ear Cleaning',               'Add-On',    12.00,  15,  1),
  ('Teeth Brushing',             'Add-On',    15.00,  15,  1),
  ('Flea & Tick Treatment',      'Treatment', 40.00,  30,  1),
  ('De-shedding Treatment',      'Treatment', 50.00,  45,  1),
  ('Medicated Bath',             'Treatment', 55.00,  60,  1),
  ('Cat Bath & Dry',             'Grooming',  45.00,  60,  1),
  ('Puppy First Groom',          'Grooming',  55.00,  75,  1),
  ('Anal Gland Expression',      'Add-On',    18.00,  10,  1),
  ('Paw Moisturising Treatment', 'Add-On',    20.00,  15,  1),
  ('Mobile Grooming (Premium)',  'Premium',  150.00, 180,  0);

-- ============================================================
-- 5. APPOINTMENTS
-- ============================================================
INSERT INTO [dbo].[Appointments] (CustomerID, PetID, GroomerID, AppointmentDate, TimeSlot, ServiceType, Status, Notes, ServiceID)
VALUES
  (1,  1,  3, '2026-06-01', '09:00 AM', 'Full Grooming (Large)',  'Completed', 'Customer requests no trimming on ears', 4),
  (2,  3,  4, '2026-06-01', '10:00 AM', 'Full Grooming (Small)',  'Completed', NULL, 2),
  (3,  5,  5, '2026-06-02', '09:00 AM', 'Basic Bath & Dry',       'Completed', NULL, 1),
  (4,  6,  3, '2026-06-03', '02:00 PM', 'Full Grooming (Large)',  'Completed', 'Hip issue – be gentle on hindquarters', 4),
  (6,  8,  4, '2026-06-05', '11:00 AM', 'Full Grooming (Large)',  'Completed', NULL, 4),
  (7, 10,  5, '2026-06-06', '10:00 AM', 'Cat Bath & Dry',          'Completed', NULL, 11),
  (10,14,  3, '2026-06-10', '09:00 AM', 'Basic Bath & Dry',       'Completed', NULL, 1),
  (1,  2,  5, '2026-06-25', '09:00 AM', 'Cat Bath & Dry',          'Pending',   NULL, 11),
  (3,  4,  4, '2026-06-25', '10:00 AM', 'Cat Bath & Dry',          'Pending',   NULL, 11),
  (5,  7,  3, '2026-06-26', '11:00 AM', 'Cat Bath & Dry',          'Pending',   NULL, 11),
  (8, 11,  4, '2026-06-27', '09:00 AM', 'Full Grooming (Large)',  'Pending',   NULL, 4),
  (10,13,  5, '2026-06-28', '03:00 PM', 'Basic Bath & Dry',       'Pending',   NULL, 1),
  (9, 12,  NULL,'2026-06-15','10:00 AM','Cat Bath & Dry',          'Cancelled', 'Customer no-show', 11);

-- ============================================================
-- 6. WALK-IN QUEUE
-- ============================================================
INSERT INTO [dbo].[WalkInQueue] (CustomerID, PetID, ServiceID, GroomerID, QueueDate, QueueNumber, Status, CheckInTime, ServedTime, Notes)
VALUES
  (1,  1,  4, 3, '2026-06-23', 1, 'Served',   '2026-06-23 09:05:00', '2026-06-23 09:30:00', NULL),
  (6,  8,  4, 4, '2026-06-23', 2, 'Served',   '2026-06-23 09:40:00', '2026-06-23 10:05:00', NULL),
  (3,  5,  1, 5, '2026-06-23', 3, 'Serving',  '2026-06-23 10:15:00', NULL,                  NULL),
  (8, 11,  4, 3, '2026-06-23', 4, 'Waiting',  '2026-06-23 10:30:00', NULL,                  'Called ahead'),
  (10,14,  1, NULL,'2026-06-23',5, 'Waiting',  '2026-06-23 10:50:00', NULL,                  NULL),
  (2,  3,  2, 4, '2026-06-22', 1, 'Served',   '2026-06-22 08:55:00', '2026-06-22 09:20:00', NULL),
  (7, 10, 11, 5, '2026-06-22', 2, 'Served',   '2026-06-22 10:00:00', '2026-06-22 10:25:00', NULL),
  (4,  6,  4, 3, '2026-06-22', 3, 'Served',   '2026-06-22 11:30:00', '2026-06-22 12:10:00', 'Hip issue noted');

-- ============================================================
-- 7. INVOICES
-- ============================================================
INSERT INTO [dbo].[Invoices] (CustomerID, PetID, AppointmentID, QueueID, SubTotal, DiscountAmt, TaxAmt, TotalAmount, PaymentMethod, Status, Notes, PaidAt)
VALUES
  (1,  1,  1, NULL, 110.00,  0.00, 0.00, 110.00, 'Cash',   'Paid',   NULL,                   '2026-06-01 10:30:00'),
  (2,  3,  2, NULL,  65.00,  5.00, 0.00,  60.00, 'QR',     'Paid',   'Loyalty discount 5',   '2026-06-01 11:15:00'),
  (3,  5,  3, NULL,  35.00,  0.00, 0.00,  35.00, 'Cash',   'Paid',   NULL,                   '2026-06-02 10:00:00'),
  (4,  6,  4, NULL, 125.00,  0.00, 0.00, 125.00, 'Card',   'Paid',   'Extra nail trim added','2026-06-03 03:45:00'),
  (6,  8,  5, NULL, 110.00,  0.00, 0.00, 110.00, 'QR',     'Paid',   NULL,                   '2026-06-05 12:30:00'),
  (7, 10,  6, NULL,  45.00,  0.00, 0.00,  45.00, 'Cash',   'Paid',   NULL,                   '2026-06-06 11:00:00'),
  (10,14,  7, NULL,  35.00,  0.00, 0.00,  35.00, 'Cash',   'Paid',   NULL,                   '2026-06-10 10:00:00'),
  (1,  1, NULL, 1,  110.00,  0.00, 0.00, 110.00, 'Cash',   'Paid',   NULL,                   '2026-06-23 10:00:00'),
  (6,  8, NULL, 2,  110.00, 10.00, 0.00, 100.00, 'Card',   'Paid',   'Returning customer 10% off','2026-06-23 10:30:00'),
  (3,  5, NULL, 3,   35.00,  0.00, 0.00,  35.00, 'Cash',   'Unpaid', NULL,                   NULL),
  (8, 11, NULL, 4,  110.00,  0.00, 0.00, 110.00, 'Cash',   'Unpaid', NULL,                   NULL);

-- ============================================================
-- 8. INVOICE ITEMS
-- ============================================================
INSERT INTO [dbo].[InvoiceItems] (InvoiceID, ServiceID, Description, Quantity, UnitPrice, LineTotal)
VALUES 
  (1, 4, 'Full Grooming (Large)',  1, 110.00, 110.00),
  (2, 2, 'Full Grooming (Small)',  1, 65.00, 65.00),
  (3, 1, 'Basic Bath & Dry',       1, 35.00, 35.00),
  (4, 4, 'Full Grooming (Large)',  1, 110.00, 110.00),
  (4, 5, 'Nail Trimming',          1,  15.00,  15.00),
  (5, 4, 'Full Grooming (Large)',  1, 110.00, 110.00),
  (6, 11, 'Cat Bath & Dry',        1, 45.00, 45.00),
  (7, 1, 'Basic Bath & Dry',       1, 35.00, 35.00),
  (8, 4, 'Full Grooming (Large)',  1, 110.00, 110.00),
  (9, 4, 'Full Grooming (Large)',  1, 110.00, 110.00),
  (10, 1, 'Basic Bath & Dry',      1, 35.00, 35.00),
  (11, 4, 'Full Grooming (Large)', 1, 110.00, 110.00);

-- Final Sanity Check Check Verification
SELECT 'Staff' AS [Table], COUNT(*) AS [Rows] FROM [dbo].[Staff] UNION ALL
SELECT 'Customers', COUNT(*) FROM [dbo].[Customers] UNION ALL
SELECT 'Pets', COUNT(*) FROM [dbo].[Pets] UNION ALL
SELECT 'Services', COUNT(*) FROM [dbo].[Services] UNION ALL
SELECT 'Appointments', COUNT(*) FROM [dbo].[Appointments] UNION ALL
SELECT 'WalkInQueue', COUNT(*) FROM [dbo].[WalkInQueue] UNION ALL
SELECT 'Invoices', COUNT(*) FROM [dbo].[Invoices] UNION ALL
SELECT 'InvoiceItems', COUNT(*) FROM [dbo].[InvoiceItems];