
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace MyORMLibrary
{
    public class TestContext<T> where T : class, new()
    {
        private readonly IDbConnection _dbConnection;

        public TestContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public T GetById(int id)
        {
            string query = $"SELECT * FROM {typeof(T).Name}s WHERE Id = @Id"; // Используем имя класса в качестве имени таблицы

            
                _dbConnection.Open();

                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = query ;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Map(reader);
                        }
                    }
                }
            

            return null;
        }

        private T Map(IDataReader reader)
        {
            var entity = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (reader[property.Name] != DBNull.Value)
                {
                    property.SetValue(entity, reader[property.Name]);
                }
            }

            return entity;
        }


        /*
         * var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

        */
    }
}

