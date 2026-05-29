using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace LibrarySystem.DAL
{
    /// <summary>
    /// SQL Server 연결 관리 클래스
    /// App.config의 connectionStrings에서 "LibraryDB" 키를 읽습니다.
    /// </summary>
    public static class DBHelper
    {
        // ─── 연결 문자열 (App.config에서 읽거나 직접 수정) ───────────────────
        private static string _connectionString =
            ConfigurationManager.ConnectionStrings["LibraryDB"]?.ConnectionString
            ?? "Server=localhost;Database=LibraryDB;Integrated Security=True;";

        public static string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        // ─── 연결 객체 반환 ────────────────────────────────────────────────
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // ─── 연결 테스트 ───────────────────────────────────────────────────
        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch { return false; }
        }

        // ─── ExecuteNonQuery (INSERT / UPDATE / DELETE) ────────────────────
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // ─── ExecuteNonQuery (저장 프로시저) ──────────────────────────────
        public static int ExecuteProc(string procName, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // ─── ExecuteScalar ─────────────────────────────────────────────────
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        // ─── ExecuteReader → DataTable ────────────────────────────────────
        public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}
