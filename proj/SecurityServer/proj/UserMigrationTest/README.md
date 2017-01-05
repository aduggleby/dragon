User Migration
==============

Profile migration
-----------------

Profile migration extracts profile information from WAV and adds it as claims to the Profile STS.
Existing claims are updated.

Usage
-----

* optional: clear existing profile data from the ProfileSTS database

        TRUNCATE TABLE IdentityUserLogin
        TRUNCATE TABLE IdentityUserClaim
        TRUNCATE TABLE IdentityUser

* Adapt the connection strings WAV and ProfileSts in the app.config file
* Run the test LegacyWavProfileMigrationTest::Migrate_validDbs_shouldMigrateProfiles
