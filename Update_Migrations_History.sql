-- Remove the problematic migration from history if it exists
DELETE FROM "__EFMigrationsHistory" 
WHERE "MigrationId" = '20251003093959_barcodeT';

-- Add the new migration to the history table
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251003094500_AddGeneratedBarcodesTable', '8.0.8');