
using APILibrary.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace APILibrary.Core.Extensions
{
    public static class WhereExtension
    {
        public static IQueryable<TModel> Where<TModel>(this IQueryable<TModel> source, string FilterStrValstring) where TModel : ModelBase
        {

            var queryExpr = source.Expression;
            var type = typeof(TModel);
            var parameterExpression = Expression.Parameter(type, "x");
            var constant = Expression.Constant("");
            var property = Expression.Property(parameterExpression, "lastname");
            var expression = Expression.Equal(property, constant);

            string test = FilterStrValstring;

            //http://xxxxx.com/catalog/v1/products?name=pizza,pates&rating=4,5&days=sunda
            //FilterStrValstring = "lastname=Guyzo,jean&email&firstname";
            var FilterValues = FilterStrValstring.Trim().Split('&').Select(x => x.Trim()).ToList();
            /*
             * Use cases
             * (rating=[4,10]) the value contain in this sequence
             * (rating=[,10])  the values less or equal than 10
             * (rating=[4,])   tht value greater or equal than 4
             */
            // equal Case

            foreach (var FilterProperty in FilterValues)
            {

                var Tosplit = FilterProperty.Split("=");
                if (Tosplit.Length <= 1)
                    continue;

                var propertyToincludeInFilter = Tosplit[0];//property to Filter
                string fieldsTofilter = Tosplit[1];//table of properties
                
                if(fieldsTofilter.Trim().StartsWith("[") && fieldsTofilter.Trim().EndsWith("]"))
                { //case value in []

                    var item = new StringBuilder(fieldsTofilter);
                    item.Replace("[","");
                    item.Replace("]", "");
                    string item2 = item.ToString().Trim();
                    var value = item2.Split(",");



                    var val1 = value[0].Equals("")? "0": value[0]; // check this case [,ConstSecond]
                    var val2 = value[1].Equals("") ? value[0] : value[1];
                    // check this Case ["",""] empty set
                    if (val1.Equals("") && val2.Equals(""))
                        continue;

                    //value of set example [Constfirst,ConstSecond]
                    var Constfirst = Expression.Constant(Int32.Parse(val1), typeof(int));
                    var ConstSecond = Expression.Constant(Int32.Parse(val2), typeof(int));
                    var property3 = Expression.Property(parameterExpression, propertyToincludeInFilter);


                    var expressionFirst = Expression.GreaterThanOrEqual(property3, Constfirst);
                    var expressionSecond = Expression.LessThanOrEqual(property3, ConstSecond);

                    expression = Expression.And(expressionSecond, expressionFirst);
                    //expression = Expression.Or(expression,expressionSecond);
                }
                else {//case equal
                foreach( var item in fieldsTofilter.Split(","))
                    {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    constant = Expression.Constant(item);
                    var property2 = Expression.Property(parameterExpression, propertyToincludeInFilter);
                    var expression2 = Expression.Equal(property2, constant);
                    expression = Expression.Or(expression, expression2);
                    }
                }

            }// End equal case

            var lambda = Expression.Lambda<Func<TModel,bool>>(expression, parameterExpression);
            return source.Where(lambda);

        }

        // Recherche
        public static IQueryable<TModel> Search<TModel>(this IQueryable<TModel> source, string search) where TModel : ModelBase
        {

            var queryExpr = source.Expression;
            var type = typeof(TModel);
            var parameterExpression = Expression.Parameter(type, "x");
            var constant = Expression.Constant("");
            var property = Expression.Property(parameterExpression, "lastname");
            var expression = Expression.Equal(property, constant);

            string test = search;

            //http://xxxxx/catalog/v1/products/search?name=*napoli*&type=pizza,pate&sort=rating,name
            //search = "Firstname=iso,jean&email&firstname";
            var SearchValues = search.Trim().Split('&').Select(x => x.Trim()).ToList();
            
            // equal Case

            foreach (var SearchProperty in SearchValues)
            {

                var Tosplit = SearchProperty.Split("=");
                if (Tosplit.Length <= 1)
                    continue;

                var propertyToincludeInSearch = Tosplit[0];//property to Filter
                string fieldsToSearch = Tosplit[1];//table of properties

                //case equal
                foreach (var item in fieldsToSearch.Split(","))
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    constant = Expression.Constant(item);
                    var property2 = Expression.Property(parameterExpression, propertyToincludeInSearch);
                    var expression2 = Expression.Equal(property2, constant);
                    expression = Expression.Or(expression, expression2);
                }
                

            }// End equal case

            var lambda = Expression.Lambda<Func<TModel, bool>>(expression, parameterExpression);
            return source.Where(lambda);

        }
    }
}
