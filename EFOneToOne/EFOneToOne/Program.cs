using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFOneToOne
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new QuContext())
            {
                var questions = ctx.Questions.Include(x => x.Acceptor).ToList();
                foreach (var question in questions)
                {
                    Console.WriteLine(question.Name);
                }

                var qu = questions.FirstOrDefault(x => x.Id == 1);
                qu.Acceptor = new QuestionAcceptor
                {
                    Name = "Foo_Acc"
                };

                ctx.Entry<Question>(qu).State = EntityState.Modified;
                ctx.SaveChanges();

                // Remove
                var question2 = ctx.Questions.Include(x => x.Acceptor).FirstOrDefault(x => x.Id == 1);
                ctx.Entry<QuestionAcceptor>(question2.Acceptor).State = EntityState.Deleted;
                ctx.SaveChanges();
            }

            Console.ReadLine();
        }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public QuestionAcceptor Acceptor { get; set; }
    }

    public class QuestionAcceptor
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public Question Question { get; set; }
    }

    public class QuContext : DbContext
    {
        public IDbSet<Question> Questions { get; set; }
        public IDbSet<QuestionAcceptor> QuestionAcceptors { get; set; }
    }
}