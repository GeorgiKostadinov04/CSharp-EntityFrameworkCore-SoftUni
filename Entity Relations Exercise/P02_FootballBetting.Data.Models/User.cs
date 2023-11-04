

using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public virtual ICollection<Bet> Bets { get; set;}
    }
}
