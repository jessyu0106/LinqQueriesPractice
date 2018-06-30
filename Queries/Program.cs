using System;
using System.Linq;

namespace Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new PlutoContext();
            /***<<Restriction>>***/
            var query = from c in context.Courses
                        where c.Level == 1 && c.Author.Id == 1 //filtering and multi-condition
                        select c; //the last line should always be select
            //Extension
            var courses1 = context.Courses.Where(c => c.Level == 1);//delegate Func lambda expression

            /***Ording***/
            var query2 = from c in context.Courses
                         where c.Author.Id == 1
                         orderby c.Level descending, c.Name //orderby multi-column
                         select c;
            //Extension
            var courses2 = context.Courses.Where(c => c.Level == 1)
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Level);
            //Descending
            courses2 = context.Courses.Where(c => c.Level == 1)
                .OrderByDescending(c => c.Name)
                .ThenByDescending(c => c.Level);

            /***Projection***/
            //project course class into different class that has a name and author
            var query3 = from c in context.Courses
                         where c.Author.Id == 1 //filtering and multi-condition
                         orderby c.Level descending, c.Name //order by multi-column| descending
                         select new { Name = c.Name, Author = c.Author.Name }; //select new object <<projection>>
            //Extension
            var courses3 = context.Courses.Where(c => c.Level == 1)
               .OrderBy(c => c.Name)
               .ThenBy(c => c.Level)
               .Select(c => new { CoutseName = c.Name, AuthorName = c.Author.Name });//anonymous object

            var listOfTags = context.Courses.Where(c => c.Level == 1)
               .OrderBy(c => c.Name)
               .ThenBy(c => c.Level)
               .Select(c => c.Tags); //IQueryable -> IEnumerable like a list
            //a list of lists of tags
            //It's a hierarchical object
            foreach (var c in listOfTags)
            {
                foreach (var tag in c)
                {
                    Console.WriteLine(tag.Name);
                }
            }

            var tags = context.Courses.Where(c => c.Level == 1)
               .OrderBy(c => c.Name)
               .ThenBy(c => c.Level)
               .SelectMany(c => c.Tags); //use select Many to flatten the list of lists
            //return flat list of tags
            foreach (var t in tags)
            {
                Console.WriteLine(t.Name);
            }

            /***Set Operators***/
            //Extension
            var tags2 = context.Courses.Where(c => c.Level == 1)
               .OrderBy(c => c.Name)
               .ThenBy(c => c.Level)
               .SelectMany(c => c.Tags)
               .Distinct();

            //return distinct list of tags
            foreach (var t in tags)
            {
                Console.WriteLine(t.Name);
            }

            /***Grouping***/
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

            //Extension
            var groups = context.Courses.GroupBy(c => c.Level);//key : level, break down by Level
            foreach (var group in groups)
            {
                Console.WriteLine("Key: " + group.Key);
                foreach (var course in group)
                {
                    Console.WriteLine("\t" + course.Name);
                }
            }

            /***Joining***/
            /***Inner join***/
            var query6 = from c in context.Courses
                         select new { CourseName = c.Name, AuthorName = c.Author.Name };
            //projection | anonymous object
            //c.Author <- navigation property
            //no need for join because here we have navigation property
            //LINQ provider and run time would auto translate this into an inner join in SQL

            var query7 = from c in context.Courses
                         join a in context.Authors on c.AuthorId equals a.Id //inner join
                         select new { CourseName = c.Name, AuthorName = a.Name };

            //Extension
            var courses7 = context.Courses.Join(context.Authors,
                c => c.AuthorId,
                a => a.Id,
                (course, author) => new
                    {
                        CourseName = course.Name,
                        AuthorName = author.Name
                    });

            /***Group Join***/
            var query8 = from a in context.Authors
                         join c in context.Courses on a.Id equals c.AuthorId // <- inner join
                         into g //<- this become a group join
                         select new { AuthorName = a.Name, Courses = g.Count() };
            //matching left side a to one or more on right side c
            foreach (var c in query8)
            {
                Console.WriteLine("{0} ({1})", c.AuthorName, c.Courses);
            }

            //Extension
            var courses8 = context.Authors.GroupJoin(context.Courses,
                a => a.Id,
                c => c.AuthorId,
                (author, courses) => new
                {
                    Author = author,
                    Courses = courses.Count()
                });

            /***Cross Join***/
            var query9 = from a in context.Authors
                         from c in context.Courses //<-cross join
                         select new { AuthorName = a.Name, CourseName = c.Name };
            foreach (var x in query9)
            {
                Console.WriteLine("{0} ({1})", x.AuthorName, x.CourseName);
            }

            //Extension
            var courses9 = context.Authors.SelectMany(a => context.Courses,
                (author, course) => new
                {
                    AuthorName = author.Name,
                    CourseName = course.Name
                });

            /***Partitioning***/
            //usesful when you want to return a page of records
            //imagine you want to display courses in pages and the size of each page i 10
            //get courses in 2nd page
            var secondPage = context.Courses.Skip(10).Take(10);

            /***Element Operators***/
            //only want single object or first object in the list
            var firstObject = context.Courses.OrderBy(x => x.Level).First();
            firstObject = context.Courses.OrderBy(x => x.Level).FirstOrDefault();//in case there's no record in source
            firstObject = context.Courses.OrderBy(x => x.Level).FirstOrDefault(c => c.FullPrice > 100);
            var lastObject = context.Courses.OrderBy(x => x.Level).Last();
            //last cannot be applied when you are working with a database like sql server
            //linq can be use with different data source like sql server onkects and memory and so on
            //not all of them can be transfer into SQL
            //if you want last record with sql server then orderby desc way and select the first one
            var singleObject = context.Courses.Single(c => c.Id == 1); 
            singleObject = context.Courses.SingleOrDefault (c => c.Id == 1);
           
            /***Quantifying***/
            var allCourses = context.Courses.All(x => x.FullPrice > 10);
            bool anyCourses = context.Courses.Any(x => x.Level == 1);

            /***Aggregating***/
            var count = context.Courses.Count();
            var maxPrice = context.Courses.Max(c => c.FullPrice);
            var minPrice = context.Courses.Min(c => c.FullPrice);
            var avgPrice = context.Courses.Average(c => c.FullPrice);

        }
    }
}
