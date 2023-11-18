using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public static class DatabaseManager
{
    private const string fileName = "element.bytes";
    private static string DBPath;
    private static SqliteConnection connection;
    private static SqliteCommand command;

    static void MyDataBase(){
        DBPath = GetDatabasePath();
    }
    // Возвращает путь к БД.
    private static string GetDatabasePath(){
        #if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, fileName);
        // #elif UNITY_STANDALONE
        //     string filePath = Path.Combine(Application.dataPath, fileName);
        //     if(!File.Exists(filePath)) UnpackDatabase(filePath);
        //     return filePath;
        #endif
    }
    // Распаковывает базу данных в указанный путь.  
    private static void UnpackDatabase(string toPath){
        string fromPath = Path.Combine(Application.streamingAssetsPath, fileName);

        WWW reader = new WWW(fromPath);
        while (!reader.isDone) { }

        File.WriteAllBytes(toPath, reader.bytes);
    }
    // Этот метод открывает подключение к БД.
    private static void OpenConnection()
    {
        connection = new SqliteConnection("Data Source=" + DBPath);
        command = new SqliteCommand(connection);
        connection.Open();
    }

    // Этот метод закрывает подключение к БД.
    public static void CloseConnection()
    {
        connection.Close();
        command.Dispose();
    }
    // Этот метод выполняет запрос query и возвращает ответ запроса.
    public static string ExecuteQueryWithAnswer(string query)
    {
        OpenConnection();
        command.CommandText = query;
        var answer = command.ExecuteScalar();
        CloseConnection();

        if (answer != null) return answer.ToString();
        else return null;
    }
    // Этот метод возвращает таблицу, которая является результатом выборки запроса query.
    public static DataTable GetTable(string query)
    {
        OpenConnection();

        SqliteDataAdapter adapter = new SqliteDataAdapter(query, connection);
        DataSet DS = new DataSet();
        adapter.Fill(DS);
        adapter.Dispose();

        CloseConnection();
        return DS.Tables[0];
    }

}
