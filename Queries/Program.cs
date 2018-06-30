using System;
using System.Linq;

namespace Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new PlutoContext();
            //Restriction
            var query = from c in context.Courses
                        where c.Level == 1 && c.Author.Id == 1 //filtering and multi-condition
                        select c; //the last line should always be select

            //Ording
            var query2 = from c in context.Courses
		    where c.Author.Id == 1 
		    orderby c.Level descending, c.Name //orderby multi-column
		    select c; 
            
            //Projection
            //project course class into different class that has a name and author
            var query3 = from c in context.Courses
                        where c.Author.Id == 1 //filtering and multi-condition
                        orderby c.Level descending, c.Name //order by multi-column| descending
                        select new { Name = c.Name, Author = c.Author.Name }; //select new object <<projection>>
            //Grouping
            var query4 = from c in context.Courses
                        group c by c.Level
                            into g // g is another variable
                            select g;
            //what we get from the result now is a list of group
            foreach (var group in query4)
            {
                //the level of courses it contains
                Console.WriteLine(group.Key);
                //display courses in that group
                //the group is enumerable, it's like a collection
                foreach (var course in group)
                {
                    Console.WriteLine("\t{0}", course.Name);
                }
            }
            //grouping with aggregation
            var query5 = from c in context.Courses
                        group c by c.Level
                            into g // g is another variable
                            select g;
            //what we get from the result now is a list of group
            foreach (var group in query5)
            {
                //the level of courses it contains
                Console.WriteLine("{0}({1})", group.Key, group.Count());
            }

            //joining
            //inner join
            var query6 = from c in context.Courses
				select new {CourseName = c.Name, AuthorName = c.Author.Name}; 
				//projection | anonymous object
				//c.Author <- navigation property
				//no need for join because here we have navigation property
				//LINQ provider and run time would auto translate this into an inner join in SQL

            var query7 = from c in context.Courses
				join a in context.Authors on c.AuthorId equals a.Id //inner join
				select new {CourseName = c.Name, AuthorName = a.Name};

            //Group Join
            var query8 = from a in context.Authors
				join c in context.Courses on a.Id equals c.AuthorId // <- inner join
				into g //<- this become a group join
				select new {AuthorName = a.Name, Courses = g.Count()};
				//matching left side a to one or more on right side c
	        foreach(var c in query8)
	        {
		        Console.WriteLine("{0} ({1})", c.AuthorName, c.Courses);
	        }	
		
            //Cross Join
            var query9 = from a in context.Authors
				from c in context.Courses //<-cross join
				select new {AuthorName = a.Name, CourseName = c.Name};
	        foreach(var x in query9)
	        {
                Console.WriteLine("{0} ({1})", x.AuthorName, x.CourseName);
	        }		
        }
    }
}
