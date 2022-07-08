namespace ADO.NET_Exersize
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Data.SqlClient;

    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = @"Server=DESKTOP-KF9CAVN\SQLEXPRESS;Database=MinionsDB;Integrated Security=True;Encrypt=False";

            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            // Task 01
            //CreateTablesMethod(sqlConnection);
            //string result = InsertIntoTablesMethod(sqlConnection);
            //Console.WriteLine(result);


            // Task 02 
            //Console.WriteLine(GetVillainsNames(sqlConnection));


            //Task 03
            //int villainId = int.Parse(Console.ReadLine());
            //Console.WriteLine(GetMinionsNames(sqlConnection, villainId));


            //Task 04 
            //string[] minionInfo = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            //string[] villainInfo = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            //string result = AddNewMinionToVillain(sqlConnection, minionInfo, villainInfo);
            //Console.WriteLine(result);


            //Task 05
            //string townName = Console.ReadLine();
            //Console.WriteLine(ChangeTownNameCasing(sqlConnection, townName));


            //Task 06
            //int villainId = int.Parse(Console.ReadLine());
            //Console.WriteLine(RemoveVillain(sqlConnection, villainId));


            //Task 07
            //Console.WriteLine(PrintMinionNames(sqlConnection));


            //Task 08
            //int[] minionsIds = Console.ReadLine().Split().Select(int.Parse).ToArray();
            //Console.WriteLine(IncreaseMinionAge(sqlConnection, minionsIds));


            //Task 09
            //int minionId = int.Parse(Console.ReadLine());
            //Console.WriteLine(IncreaseAgeStoredProcedure(sqlConnection, minionId));

            sqlConnection.Close();
        }

        //Task 01 Initial Setup 
        private static void CreateTablesMethod(SqlConnection sqlConnection)
        {
            string[] tableQueries = new string[]
            {
                "CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50))",
                "CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id))",
                "CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))",
                "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50))",
                "CREATE TABLE Villains (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))",
                "CREATE TABLE MinionsVillains (MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId))"
            };

            foreach (var tableQuery in tableQueries)
            {
                SqlCommand tablesCmd = new SqlCommand(tableQuery, sqlConnection);
                tablesCmd.ExecuteNonQuery();
            }
        }

        private static string InsertIntoTablesMethod(SqlConnection sqlConnection)
        {
            int totalRowsAffected = 0;
            string[] insertQueries = new string[]
            {
                "INSERT INTO Countries ([Name]) VALUES ('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway')",
                "INSERT INTO Towns ([Name], CountryCode) VALUES ('Plovdiv', 1),('Varna', 1),('Burgas', 1),('Sofia', 1),('London', 2),('Southampton', 2),('Bath', 2),('Liverpool', 2),('Berlin', 3),('Frankfurt', 3),('Oslo', 4)",
                "INSERT INTO Minions (Name,Age, TownId) VALUES('Bob', 42, 3),('Kevin', 1, 1),('Bob ', 32, 6),('Simon', 45, 3),('Cathleen', 11, 2),('Carry ', 50, 10),('Becky', 125, 5),('Mars', 21, 1),('Misho', 5, 10),('Zoe', 125, 5),('Json', 21, 1)",
                "INSERT INTO EvilnessFactors (Name) VALUES ('Super good'),('Good'),('Bad'), ('Evil'),('Super evil')",
                "INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('Gru',2),('Victor',1),('Jilly',3),('Miro',4),('Rosen',5),('Dimityr',1),('Dobromir',2)",
                "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (4,2),(1,1),(5,7),(3,5),(2,6),(11,5),(8,4),(9,7),(7,1),(1,3),(7,3),(5,3),(4,3),(1,2),(2,1),(2,7)"
            };

            foreach (var insertQuery in insertQueries)
            {
                SqlCommand insertCmd = new SqlCommand(insertQuery, sqlConnection);
                int rowsAffected = insertCmd.ExecuteNonQuery();
                totalRowsAffected += rowsAffected;
            }

            return $"Rows affected {totalRowsAffected}.";
        }

        //Task 02 Villain Names
        private static string GetVillainsNames(SqlConnection sqlConnection)
        {
            StringBuilder sb = new StringBuilder();

            string query = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                               FROM Villains AS v
                               JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                           GROUP BY v.Id, v.Name
                             HAVING COUNT(mv.VillainId) > 3
                           ORDER BY COUNT(mv.VillainId)";

            SqlCommand villainsName = new SqlCommand(query, sqlConnection);
            using SqlDataReader dataReader = villainsName.ExecuteReader();
            while (dataReader.Read())
            {
                sb.AppendLine($"{dataReader[0]} - {dataReader[1]}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 03 Minion Names
        private static string GetMinionsNames(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder sb = new StringBuilder();

            string villainQuery = @"SELECT Name FROM Villains WHERE Id = @Id";
            string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

            SqlCommand villainCmd = new SqlCommand(villainQuery, sqlConnection);
            villainCmd.Parameters.AddWithValue("@Id", villainId);

            string villainName = (string)villainCmd.ExecuteScalar();

            if (villainName == null)
            {
                sb.AppendLine($"No villain with ID {villainId} exists in the database.");
            }
            else 
            {
                sb.AppendLine($"Villain: {villainName}");

                SqlCommand minionsCmd = new SqlCommand(minionsQuery, sqlConnection);
                minionsCmd.Parameters.AddWithValue("Id", villainId);

                using SqlDataReader minionsReader = minionsCmd.ExecuteReader();

                if (minionsReader.HasRows)
                {
                    while (minionsReader.Read())
                    {
                        sb.AppendLine($"{minionsReader[0]}. {minionsReader[1]} {minionsReader[2]}");
                    }
                }
                else
                {
                    sb.AppendLine("(no minions)");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Task 04 Add Minion
        private static string AddNewMinionToVillain(
            SqlConnection sqlConnection,
            string[] minionsInfo,
            string[] villainsInfo)
        {
            StringBuilder sb = new StringBuilder();
            
            string minionName = minionsInfo[1];
            int minionAge = int.Parse(minionsInfo[2]);
            string minionTown = minionsInfo[3];

            string villainName = villainsInfo[1];

            string setMinionToVillainQuery = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                int minionTownId = GetMinionTownId(sqlConnection, sqlTransaction, sb, minionTown);
                int villainId = GetVillainId(sqlConnection, sqlTransaction, sb, villainName);
                int minionId = AddMinionAndGetHisId(sqlConnection, sqlTransaction, minionTownId, minionAge, minionName);

                SqlCommand addMinionToVillain = new SqlCommand(setMinionToVillainQuery, sqlConnection, sqlTransaction);
                addMinionToVillain.Parameters.AddWithValue("@villainId", villainId);
                addMinionToVillain.Parameters.AddWithValue("@minionId", minionId);

                addMinionToVillain.ExecuteScalar();

                sb.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

                sqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                return ex.ToString();
            }

            return sb.ToString().TrimEnd();
        }

        private static int GetMinionTownId(
            SqlConnection sqlConnection, 
            SqlTransaction sqlTransaction, 
            StringBuilder sb, 
            string townName)
        {
            string townQuery = @"SELECT Id FROM Towns WHERE Name = @townName";
            string insertTownQuery = @"INSERT INTO Towns (Name) VALUES (@townName)";

            SqlCommand townCmd = new SqlCommand(townQuery, sqlConnection, sqlTransaction);
            townCmd.Parameters.AddWithValue("@townName", townName);

            object townID = townCmd.ExecuteScalar();

            if (townID == null)
            {
                SqlCommand insertTownCmd = new SqlCommand(insertTownQuery, sqlConnection, sqlTransaction);
                insertTownCmd.Parameters.AddWithValue("@townName", townName);

                insertTownCmd.ExecuteNonQuery();

                sb.AppendLine($"Town {townName} was added to the database.");

                townID = townCmd.ExecuteScalar();
            }

            return (int)townID;
        }

        private static int GetVillainId(
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction,
            StringBuilder sb,
            string villainName)
        {
            string villainQuery = @"SELECT Id FROM Villains WHERE Name = @Name";
            string insertVillaintQuery = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";

            SqlCommand villainCmd = new SqlCommand(villainQuery, sqlConnection, sqlTransaction);
            villainCmd.Parameters.AddWithValue("@Name", villainName);

            object villaintId = villainCmd.ExecuteScalar();

            if (villaintId == null)
            {
                SqlCommand insertVillainCmd = new SqlCommand(insertVillaintQuery, sqlConnection, sqlTransaction);
                insertVillainCmd.Parameters.AddWithValue("@villainName", villainName);

                insertVillainCmd.ExecuteScalar();

                sb.AppendLine($"Villain {villainName} was added to the database.");

                villaintId = villainCmd.ExecuteScalar();
            }

            return (int)villaintId;
        }

        private static int AddMinionAndGetHisId(
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction,
            int townId,
            int age,
            string minionName)
        {
            string minionQuery = @"SELECT [Id]
                                     FROM [Minions]
                                    WHERE [Name] = @Name AND [Age] = @Age AND [TownId] = @TownId";
            string insertMinionQuery = @"INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";

            SqlCommand minionCmd = new SqlCommand(minionQuery, sqlConnection, sqlTransaction);
            minionCmd.Parameters.AddWithValue("@Name", minionName);
            minionCmd.Parameters.AddWithValue("@Age", age);
            minionCmd.Parameters.AddWithValue("@TownId", townId);

            object minionId = minionCmd.ExecuteScalar();

            if (minionId == null)
            {
                SqlCommand insertMinionCmd = new SqlCommand(insertMinionQuery, sqlConnection, sqlTransaction);
                insertMinionCmd.Parameters.AddWithValue("@name", minionName);
                insertMinionCmd.Parameters.AddWithValue("@age", age);
                insertMinionCmd.Parameters.AddWithValue("@townId", townId);

                insertMinionCmd.ExecuteScalar();

                minionId = minionCmd.ExecuteScalar();
            }

            return (int)minionId;
        }

        //Task 05 Change Town Names Casing
        private static string ChangeTownNameCasing(SqlConnection sqlConnection, string countryName)
        {
            StringBuilder sb = new StringBuilder();

            string updateQuery = @"UPDATE Towns
                                      SET Name = UPPER(Name)
                                    WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";
            string townNamesQuery = @"SELECT [t].[Name]
                                        FROM [Countries] AS [c]
                                        JOIN [Towns] AS [t] ON [c].[Id]= [t].[CountryCode]
                                       WHERE [c].[Name] = @countryName";

            SqlCommand updateTownsCmd = new SqlCommand(updateQuery, sqlConnection);
            updateTownsCmd.Parameters.AddWithValue("@countryName", countryName);

            int townsAffected = updateTownsCmd.ExecuteNonQuery();

            if (townsAffected == 0)
            {
                return "No town names were affected.";
            }
            else
            {
                sb.AppendLine($"{townsAffected} town names were affected.");
            }

            SqlCommand townNamesCmd = new SqlCommand(townNamesQuery, sqlConnection);
            townNamesCmd.Parameters.AddWithValue("@countryName", countryName);

            using SqlDataReader dataReader = townNamesCmd.ExecuteReader();
            List<string> citiesNames = new List<string>();

            while (dataReader.Read())
            {
                citiesNames.Add($"{dataReader["Name"]}");
            }

            sb.AppendLine(String.Join(", ", citiesNames));

            return sb.ToString().TrimEnd();
        }

        //Task 06 Remove Villain 
        private static string RemoveVillain(SqlConnection sqlConnection, int villainId)
        {

            string getVillainNameQuery = @"SELECT Name FROM Villains WHERE Id = @villainId";
            string deleteMinionsVillainsQuery = @"DELETE FROM MinionsVillains 
                                                        WHERE VillainId = @villainId";
            string deleteVillainQuery = @"DELETE FROM Villains
                                                WHERE Id = @villainId";

            SqlTransaction transaction = sqlConnection.BeginTransaction();
            try
            {
                SqlCommand villainNameCmd = new SqlCommand(getVillainNameQuery, sqlConnection, transaction);
                villainNameCmd.Parameters.AddWithValue("@villainId", villainId);

                string villainName = (string)villainNameCmd.ExecuteScalar();

                if (villainName == null)
                {
                    return "No such villain was found.";
                }

                SqlCommand deleteFromMinionsVillainsCmd = new SqlCommand(deleteMinionsVillainsQuery, sqlConnection, transaction);
                deleteFromMinionsVillainsCmd.Parameters.AddWithValue("@villainId", villainId);
                int minionsCount = deleteFromMinionsVillainsCmd.ExecuteNonQuery();

                SqlCommand deleteVillainCmd = new SqlCommand(deleteVillainQuery, sqlConnection, transaction);
                deleteVillainCmd.Parameters.AddWithValue("@villainId", villainId);
                deleteVillainCmd.ExecuteNonQuery();

                Console.WriteLine($"{villainName} was deleted.");
                Console.WriteLine($"{minionsCount} minions were released.");

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ex.ToString();
            }

            return null;
        }

        //Task 07 Print All Minion Names
        private static string PrintMinionNames(SqlConnection sqlConnection)
        {
            string minionNamesQuery = @"SELECT Name FROM Minions";
            
            List<string> names = new List<string>();
            List<string> newNames = new List<string>();

            SqlCommand getMinionNamesCmd = new SqlCommand(minionNamesQuery, sqlConnection);
            using SqlDataReader dataReader = getMinionNamesCmd.ExecuteReader();

            while (dataReader.Read())
            {
                names.Add($"{dataReader["Name"]}");
            }

            if (names.Count % 2 == 0)
            {
                for (int i = 0; i < names.Count / 2; i++)
                {
                    newNames.Add(names[0 + i]);
                    newNames.Add(names[names.Count - 1 - i]);
                }
            }
            else
            {
                for (int i = 0; i < Math.Floor(names.Count / 2.0) + 1; i++)
                {
                    if (i == Math.Floor(names.Count / 2.0))
                    {
                        newNames.Add(names[0 + i]);
                    }
                    else
                    {
                        newNames.Add(names[0 + i]);
                        newNames.Add(names[names.Count - 1 - i]);
                    }
                }
            }

            newNames.ForEach(n => Console.WriteLine(n));

            return null;
        }

        //Task 08 Increase Minion Age
        private static string IncreaseMinionAge(SqlConnection sqlConnection, int[] minionsId)
        {
            StringBuilder sb = new StringBuilder();
            string updateNameAndAgeQuery = @"UPDATE Minions
                                                SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                              WHERE Id = @Id";
            string getMinionsNames = @"SELECT Name, Age FROM Minions";

            for (int i = 0; i < minionsId.Length; i++)
            {
                int currMinionId = minionsId[i];

                SqlCommand updateCmd = new SqlCommand(updateNameAndAgeQuery, sqlConnection);
                updateCmd.Parameters.AddWithValue("@Id", currMinionId);

                updateCmd.ExecuteNonQuery();
            }

            SqlCommand getNamesCmd = new SqlCommand(getMinionsNames, sqlConnection);
            using SqlDataReader dataReader = getNamesCmd.ExecuteReader();

            while (dataReader.Read())
            {
                sb.AppendLine($"{dataReader["Name"]} {dataReader["Age"]}");
            }

            return sb.ToString().TrimEnd();
        }

        //Task 09 Increase Age Stored Procedure 
        private static string IncreaseAgeStoredProcedure(SqlConnection sqlConnection, int minionId)
        {
            StringBuilder sb = new StringBuilder();

            string minionQuery = @"SELECT Name, Age FROM Minions WHERE Id = @Id";
            string execProcedureQuery = @"EXEC usp_GetOlder @id";

            SqlCommand execProcCmd = new SqlCommand(execProcedureQuery, sqlConnection);
            execProcCmd.Parameters.AddWithValue("@Id", minionId);
            int rowsAffected = execProcCmd.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                return $"No minion with ID {minionId} exist in DateBase.";
            }

            SqlCommand getMinionNameCmd = new SqlCommand(minionQuery, sqlConnection);
            getMinionNameCmd.Parameters.AddWithValue("@Id", minionId);

            using SqlDataReader dataReader = getMinionNameCmd.ExecuteReader();
            while (dataReader.Read())
            {
                sb.AppendLine($"{dataReader["Name"]} – {dataReader["Age"]} years old");
            }

            return sb.ToString().TrimEnd();
        }

    }
}
