using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DbTransactions101
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Ref: http://www.codeproject.com/Articles/522039/A-Beginners-Tutorial-for-Understanding-Transaction
            //      http://msdn.microsoft.com/en-us/magazine/cc300805.aspx

            // T-SQL Ref:
            // See all databases: SELECT * FROM sys.databases
            // See the running transactions for the DB: SELECT * FROM [FooBank].sys.dm_tran_database_transactions;
            // Using TRY...CATCH in Transact-SQL: http://msdn.microsoft.com/en-us/library/ms179296(v=sql.105).aspx
            // Deadlocking: http://msdn.microsoft.com/en-us/library/ms177433(v=sql.105).aspx
            // Minimizing Deadlocks: http://msdn.microsoft.com/en-us/library/ms191242(v=sql.105).aspx

            PerformTransactionAsync(1, 2, 6767.90f);
            PerformTransactionWithTransactionScopeAsync(1, 2, 6767.90f);

            Console.ReadLine();
        }

        private static async Task PerformTransactionWithTransactionScopeAsync(int creditAccountId, int debitAccountId, float amount)
        {
            const string debitSqlStatement = "UPDATE Accounts SET Amount = Amount - @creditAmount WHERE ID = @accountId";
            const string creditSqlStatement = "UPDATE Accounts SET Amount = Amount + @creditAmount WHERE ID = @accountId";

            using (SqlConnection con = new SqlConnection(GetAsyncConnectionString()))
            {
                await con.OpenAsync();

                bool debitResult = false;
                bool creditResult = false;

                // begin a transaction here
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand(debitSqlStatement, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@accountId", SqlDbType.Int) { Value = debitAccountId });
                        cmd.Parameters.Add(new SqlParameter("@creditAmount", SqlDbType.Decimal) { Value = amount, Precision = 18, Scale = 2 });

                        cmd.Transaction = transaction;

                        try
                        {
                            int result = await cmd.ExecuteNonQueryAsync();
                            debitResult = result == 1;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception occured! Message: {0}", ex.Message);
                        }
                    }

                    // A dummy throw just to check whether the transaction are working or not
                    //throw new Exception("Let see..."); // uncomment this line to see the transaction in action

                    using (SqlCommand cmd = new SqlCommand(creditSqlStatement, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@accountId", SqlDbType.Int) { Value = creditAccountId });
                        cmd.Parameters.Add(new SqlParameter("@creditAmount", SqlDbType.Decimal) { Value = amount, Precision = 18, Scale = 2 });

                        cmd.Transaction = transaction;

                        try
                        {
                            int result = await cmd.ExecuteNonQueryAsync();
                            creditResult = result == 1;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception occured! Message: {0}", ex.Message);
                        }
                    }

                    if (debitResult && creditResult)
                    {
                        transaction.Commit();
                    }
                }
            }
        }

        private static async Task PerformTransactionAsync(int creditAccountId, int debitAccountId, float amount)
        {
            const string sqlStatement = @"BEGIN TRY
                                          BEGIN TRANSACTION
                                              UPDATE Accounts SET Amount = Amount - @creditAmount WHERE ID = @debitAccountId
                                              UPDATE Accounts SET Amount = Amount + @creditAmount WHERE ID = @creditAccountId
                                          COMMIT TRANSACTION
                                          END TRY
                                          BEGIN CATCH
                                              ROLLBACK TRANSACTION
                                              -- Call the procedure to raise the original error.
                                              EXEC usp_RethrowError;
                                          END CATCH";

            string connectionString = ConfigurationManager.ConnectionStrings["AccountsDb"].ConnectionString;
            string asyncConnectionString = new SqlConnectionStringBuilder(connectionString) { AsynchronousProcessing = true }.ToString();

            using (SqlConnection con = new SqlConnection(asyncConnectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(sqlStatement, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@debitAccountId", SqlDbType.Int) { Value = debitAccountId });
                    cmd.Parameters.Add(new SqlParameter("@creditAccountId", SqlDbType.Int) { Value = creditAccountId });
                    cmd.Parameters.Add(new SqlParameter("@creditAmount", SqlDbType.Decimal) { Value = amount, Precision = 18, Scale = 2 });

                    try
                    {
                        int result = await cmd.ExecuteNonQueryAsync();

                        // check if 2 records are effected or not
                        bool isSuccess = result == 2;
                        if (isSuccess)
                        {
                            Console.WriteLine("Successful TRANSACTION!");
                        }
                        else
                        {
                            Console.WriteLine("Unsuccessful TRANSACTION!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception occured! Message: {0}", ex.Message);
                    }
                }
            }
        }

        private static string GetAsyncConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AccountsDb"].ConnectionString;
            string asyncConnectionString = new SqlConnectionStringBuilder(connectionString) { AsynchronousProcessing = true }.ToString();
            return asyncConnectionString;
        }
    }
}