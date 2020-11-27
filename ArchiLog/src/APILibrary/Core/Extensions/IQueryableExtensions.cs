using APILibrary.Core.Attributes;
using APILibrary.Core.Filters;
using APILibrary.Core.Models;
using Microsoft.EntityFrameworkCore;
using StringToExpression.LanguageDefinitions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static APILibrary.Core.Filters.FilterUtility;

namespace APILibrary.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static object SelectObject(object value, string[] fields)
        {
            var expo = new ExpandoObject() as IDictionary<string, object>;
            var collectionType = value.GetType();

            foreach (var field in fields)
            {
                var prop = collectionType.GetProperty(field, BindingFlags.Public |
                    BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (prop != null)
                {
                    var isPresentAttribute = prop.CustomAttributes
                         .Any(x => x.AttributeType == typeof(NotJsonAttribute));
                    if (!isPresentAttribute)
                        expo.Add(prop.Name, prop.GetValue(value));
                }
                else
                {
                    throw new Exception($"Property {field} does not exist.");
                }
            }
            return expo;
        }

        public static IQueryable<dynamic> SelectDynamic<TModel>(this IQueryable<TModel> query, string[] fields) where TModel : ModelBase
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");

            var membersExpression = fields.Select(y => Expression.Property(parameter, y));

            var membersAssignment = membersExpression.Select(z => Expression.Bind(z.Member, z));

            var body = Expression.MemberInit(Expression.New(typeof(TModel)), membersAssignment);

            var lambda = Expression.Lambda<Func<TModel, dynamic>>(body, parameter);


            return query.Select(lambda);
        }

        public static IQueryable<TModel> SelectModel<TModel>(this IQueryable<TModel> query, string[] fields) where TModel : ModelBase
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");

            var membersExpression = fields.Select(y => Expression.Property(parameter, y));

            var membersAssignment = membersExpression.Select(z => Expression.Bind(z.Member, z));

            var body = Expression.MemberInit(Expression.New(typeof(TModel)), membersAssignment);

            var lambda = Expression.Lambda<Func<TModel, TModel>>(body, parameter);

            return query.Select(lambda);
        }



        public static IQueryable<TModel> Filtera<TModel>(this IQueryable<TModel> query, string filterColumn, string filterValue)
        {
            var parameterExpression = Expression.Parameter(typeof(TModel), "x");
            var constant = Expression.Constant(filterValue);
            var property = Expression.Property(parameterExpression, filterColumn);
            var expression = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TModel, bool>>(expression, parameterExpression);

            query = query.Where(lambda);
            return query;
        }

        /*public static IQueryable<TModel> ApplyFilter<TModel>(this IQueryable<TModel> query, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return query;
            }

            try
            {
                var compiledFilter = new ODataFilterLanguage().Parse<TModel>(filter);

                return query.Where(compiledFilter);
            }
            catch (Exception e)
            {
                throw new FormatException($"Provided filter expression '{filter}' has incorrect format", e);
            }
        }*/

        /*public static IQueryable<TModel> FilteredData<TModel>(this IQueryable<TModel> data, IEnumerable<FilterParams> filterParams) where TModel : ModelBase
        {

            IEnumerable<string> distinctColumns = filterParams.Where(x => !String.IsNullOrEmpty(x.ColumnName)).Select(x => x.ColumnName).Distinct();
            //IEnumerable<string> distinctColumns = filterParams.Select(x => x.ColumnName).Distinct();

            foreach (var colName in distinctColumns)
            {
                var filterColumn = typeof(TModel).GetProperty(colName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (filterColumn != null)
                {
                    IEnumerable<FilterParams> filterValues = filterParams.Where(x => x.ColumnName.Equals(colName)).Distinct();

                    if (filterValues.Count() > 1)
                    {
                        IEnumerable<TModel> sameColData = Enumerable.Empty<TModel>();

                        foreach (var val in filterValues)
                        {
                            sameColData = sameColData.Concat(FilterData(val.FilterOption, data, filterColumn, val.FilterValue));
                        }

                        //data = data.Where(x => x.ID.Equals(sameColData.Select(y => y.ID)));

                        data = data.Intersect(sameColData);
                    }
                    else
                    {
                        data = data.Intersect(FilterData(filterValues.FirstOrDefault().FilterOption, data, filterColumn, filterValues.FirstOrDefault().FilterValue));
                    }
                }
            }
            return data;
        }*/

        /*private static IEnumerable<TModel> FilterData<TModel>(FilterOptions filterOption, IEnumerable<TModel> data, PropertyInfo filterColumn, string filterValue) where TModel : ModelBase
        {
            int outValue;
            DateTime dateValue;
            switch (filterOption)
            {
                #region [StringDataType]  

                case FilterOptions.StartsWith:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower().StartsWith(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.EndsWith:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower().EndsWith(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.Contains:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower().Contains(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.DoesNotContain:
                    data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                     (filterColumn.GetValue(x, null) != null && !filterColumn.GetValue(x, null).ToString().ToLower().Contains(filterValue.ToString().ToLower()))).ToList();
                    break;
                case FilterOptions.IsEmpty:
                    data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                     (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString() == string.Empty)).ToList();
                    break;
                case FilterOptions.IsNotEmpty:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString() != string.Empty).ToList();
                    break;
                #endregion

                #region [Custom]  

                case FilterOptions.IsGreaterThan:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) > outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) > dateValue).ToList();

                    }
                    break;

                case FilterOptions.IsGreaterThanOrEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) >= outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) >= dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsLessThan:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) < outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) < dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsLessThanOrEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) <= outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) <= dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsEqualTo:
                    if (filterValue == string.Empty)
                    {
                        data = data.Where(x => filterColumn.GetValue(x, null) == null
                                        || (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower() == string.Empty)).ToList();
                    }
                    else
                    {
                        if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                        {
                            data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) == outValue).ToList();
                        }
                        else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                        {
                            data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) == dateValue).ToList();
                            break;
                        }
                        else
                        {
                            data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower() == filterValue.ToLower()).ToList();
                        }
                    }
                    break;

                case FilterOptions.IsNotEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) != outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) != dateValue).ToList();
                        break;
                    }
                    else
                    {
                        data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                         (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower() != filterValue.ToLower())).ToList();
                    }
                    break;
                    #endregion
            }
            return data;
        }*/



    }
}