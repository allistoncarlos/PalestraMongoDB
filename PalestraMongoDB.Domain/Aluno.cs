using System.Collections.Generic;

namespace PalestraMongoDB.Domain
{
    public class Aluno : Entity
    {
        public string Nome { get; set; }

        public string Email { get; set; }

		public List<string> DisciplinasId { get; set; }
    }
}