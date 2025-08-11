namespace app.Helpers
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            // Implementar lógica de hash de senha
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string password, string hashedPassword)
        {
            // Verificar se a senha corresponde ao hash
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}