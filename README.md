This is a Transaction System simulation project. It supports the following operations.
1. Create account
2. Delete account
3. Get account by id
4. List all accounts
5. Deposit money to account
6. Withdraw money from account
7. Transfer money between accounts.
It uses a simple in memory dictionary data structure for data store. Asynchronous behavior is simulated together with thread safety operations.
It supports both console app and web api projects for testing. Unit tests are added.
Future improvements:
1. Use real database for data store.
2. Add authentication for api project.
3. Use immutable contracts for api communication(do not use current data models)
4. Use logging.
5. Use response pattern between layers and more detailed error handling and logging.
6. Add integration tests with docker container.
