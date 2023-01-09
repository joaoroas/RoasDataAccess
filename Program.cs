using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using RoasDataAccess.Models;

const string connectionString = "Server=localhost;Database=Roas;Trusted_Connection=True;Encrypt=False";

using (var connection = new SqlConnection(connectionString))
{
    // UTILIZANDO DAPPER
    Transaction(connection);
    //Like(connection, "api");
    //SelectIn(connection);
    //QueryMultiple(connection);
    //OneToMany(connection);
    //OneToOne(connection);
    //ReadView(connection);
    //ExecuteScalar(connection);
    //ExecuteReaderProcedure(connection);
    //ExecuteProcedure(connection);
    //CreateManyCategory(connection);
    //DeleteCategory(connection);
    //ListCategories(connection);
    //CreateCategory(connection);
    //UpdateCategory(connection);


    static void ListCategories(SqlConnection connection)
    {
        var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
        foreach (var item in categories)
        {
            Console.WriteLine($"{item.Id} - {item.Title}");
        }
    }

    static void CreateCategory(SqlConnection connection)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços da AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;


        var insertSql = @"INSERT INTO [Category] VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

        var rows = connection.Execute(insertSql, new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        });
        Console.WriteLine($"{rows} Linhas inseridas");
    }

    static void UpdateCategory(SqlConnection connection)
    {
        var updateQuery = "UPDATE [Category] SET [Title]=@Title WHERE [Id]=@Id";

        var rows = connection.Execute(updateQuery, new
        {
            Id = "af3407aa-11ae-4621-a2ef-2028b85507c4",
            Title = "Frontend"
        });
        Console.WriteLine($"{rows} Linhas alteradas");
    }

    static void DeleteCategory(SqlConnection connection)
    {
        var deleteQuery = "DELETE FROM [Category] WHERE [Id]=@Id";
        var rows = connection.Execute(deleteQuery, new
        {
            Id = "602a0d38-e85d-4958-8a05-90870832582f"
        });
        Console.WriteLine($"{rows} Registros deletados.");
    }

    static void CreateManyCategory(SqlConnection connection)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços da AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;

        var category2 = new Category();
        category2.Id = Guid.NewGuid();
        category2.Title = "Categoria nova";
        category2.Url = "categoria-nova";
        category2.Description = "Categoria nova";
        category2.Order = 9;
        category2.Summary = "Categoria";
        category2.Featured = true;


        var insertSql = @"INSERT INTO [Category] VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

        var rows = connection.Execute(insertSql, new[] {
            new
            {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
            },
            new
            {
            category2.Id,
            category2.Title,
            category2.Url,
            category2.Summary,
            category2.Order,
            category2.Description,
            category2.Featured
            }
        });
        Console.WriteLine($"{rows} Linhas inseridas");
    }

    static void ExecuteProcedure(SqlConnection connection)
    {
        var procedure = "spDeleteStudent";
        var pars = new { StudentId = "77de4535-2884-4e51-a6b9-a3094c7b3280" };
        var rows = connection.Execute(procedure, pars, commandType: CommandType.StoredProcedure);
        Console.WriteLine($"{rows} Linhas afetadas");
    }

    static void ExecuteReaderProcedure(SqlConnection connection)
    {
        var procedure = "spCoursesByCategory";
        var pars = new { CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142" };
        var courses = connection.Query(procedure, pars, commandType: CommandType.StoredProcedure);
        foreach (var course in courses)
        {
            Console.WriteLine(course.Title);
            Console.WriteLine(course.Summary);
            Console.WriteLine("==================================");
        }
    }

    static void ExecuteScalar(SqlConnection connection)
    {
        var category = new Category();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços da AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;


        var insertSql = @"INSERT INTO [Category] OUTPUT inserted.[Id] VALUES (NEWID(), @Title, @Url, @Summary, @Order, @Description, @Featured)";

        var id = connection.ExecuteScalar<Guid>(insertSql, new
        {
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        });
        Console.WriteLine($"O Id da categoria inserida é: {id}");
    }

    static void ReadView(SqlConnection connection)
    {
        var sql = "SELECT * FROM [vwCourses]";
        var courses = connection.Query(sql);
        foreach (var item in courses)
        {
            Console.WriteLine($"Titulo: {item.Title}; Categoria: {item.Category}; Autor: {item.Author}");
        }
    }

    static void OneToOne(SqlConnection connection)
    {
        var sql = @"
            SELECT
                *
            FROM
                [CareerItem]
            INNER JOIN
                [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

        var items = connection.Query<CareerItem, Course, CareerItem>(
            sql,
            (careerItem, course) =>
            {
                careerItem.Course = course;
                return careerItem;
            }, splitOn: "Id");
        foreach (var item in items)
        {
            Console.WriteLine($"{item.Title}; Curso - {item.Course.Title}");
        }
    }

    static void OneToMany(SqlConnection connection)
    {
        var sql = @"
        SELECT 
            [Career].[Id],
            [Career].[Title],
            [CareerItem].[CareerId],
            [CareerItem].[Title]
        FROM 
            [Career] 
        INNER JOIN 
            [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
        ORDER BY
            [Career].[Title]";

        var careers = new List<Career>();
        var items = connection.Query<Career, CareerItem, Career>(
            sql,
            (career, item) =>
            {
                var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                if (car == null)
                {
                    car = career;
                    car.Items.Add(item);
                    careers.Add(car);
                }
                else
                {
                    car.Items.Add(item);
                }
                return career;
            }, splitOn: "CareerId");
        foreach (var career in careers)
        {
            Console.WriteLine($"{career.Title}");
            foreach (var item in career.Items)
            {
                Console.WriteLine($"{item.Title}");
            }
        }
    }

    static void QueryMultiple(SqlConnection connection)
    {
        var query = "SELECT * FROM [Category]; SELECT * FROM [Course]";
        using var multi = connection.QueryMultiple(query);
        {
            var categories = multi.Read<Category>();
            var courses = multi.Read<Course>();

            foreach (var item in categories)
            {
                Console.WriteLine(item.Title);
            }
            foreach (var course in courses)
            {
                Console.WriteLine(course.Title);
            }
        }
    }

    static void SelectIn(SqlConnection connection)
    {
        var query = @"SELECT * FROM [Career] WHERE [Id] IN @Id";

        var items = connection.Query<Career>(query, new
        {
            Id = new[]{
                "4327ac7e-963b-4893-9f31-9a3b28a4e72b",
                "92d7e864-bea5-4812-80cc-c2f4e94db1af"
            }
        });

        foreach (var item in items)
        {
            Console.WriteLine(item.Title);
        }
    }

    static void Like(SqlConnection connection, string term)
    {
        var query = @"SELECT * FROM [Course] WHERE [Title] LIKE @exp";

        var items = connection.Query<Course>(query, new
        {
            exp = $"%{term}%"
        });

        foreach (var item in items)
        {
            Console.WriteLine(item.Title);
        }
    }

    static void Transaction(SqlConnection connection)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Title = "Não quero salvar";
        category.Url = "não";
        category.Description = "Categoria que não quero salvar";
        category.Order = 10;
        category.Summary = "não salvar";
        category.Featured = false;


        var insertSql = @"INSERT INTO [Category] VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";
        connection.Open();
        using var transaction = connection.BeginTransaction();
        {
            var rows = connection.Execute(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            }, transaction);
            //transaction.Commit();
            transaction.Rollback();
            Console.WriteLine($"{rows} Linhas inseridas");
        }
    }
}
// UTILIZANDO ADO.NET
/* Console.WriteLine("Conectado ao banco");
connection.Open();

using (var command = new SqlCommand())
{
    command.Connection = connection;
    command.CommandType = System.Data.CommandType.Text;
    command.CommandText = "SELECT [Id], [Title] FROM [Category]";

    var reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
    }

} */
