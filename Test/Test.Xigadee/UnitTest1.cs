#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xigadee;
namespace Test.Xigadee
{
    [TestClass]
    public class Entities
    {
        public Func<TSource, object> GenericEvaluateOrderBy<TSource>(string propertyName)
        {
            var type = typeof(TSource);
            var parameter = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(parameter,
                    propertyName);
            return Expression.Lambda<Func<TSource, object>>
                    (propertyReference, new[] { parameter }).Compile();
        }

        public IQueryable<TSource> GenericEvaluateOrderBy<TSource>(IQueryable<TSource> query, string propertyName)
        {
            var type = typeof(TSource);
            var parameter = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(parameter, propertyName);
            var sortExpression = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new Type[] { type },
                null,
                Expression.Lambda<Func<TSource, bool>>(propertyReference, new[] { parameter }));
            return query.Provider.CreateQuery<TSource>(sortExpression);
        }

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    var data =new List<MondayMorningBlues>();

        //    for (int i = 10; i > 0; i--)
        //    {
        //        data.Add(new MondayMorningBlues() { Email = $"pstancer+{i}@gmail.com", Message = $"Message = {i}" });
        //    }

        //    //var query = from s in data
        //    //            where s.Email == "18"
        //    //            select s.Id;

        //    Expression<Func<IEnumerable<MondayMorningBlues>, IEnumerable<MondayMorningBlues>>> something 
        //        = (s) => s.Where(r => r.Email == "freedy");

        //    var hello = something.Compile();

        //    //Expression.Equal(
        //    try
        //    {
        //        var queryableData = data.AsQueryable<MondayMorningBlues>();
        //        var value = GenericEvaluateOrderBy<MondayMorningBlues>("Email")(data[0]);
        //        //var value2 = GenericEvaluateOrderBy<MondayMorningBlues>(queryableData, "Email").ToList();

        //        var properties = typeof(MondayMorningBlues).GetProperties().Where(p => p.CanRead);

        //        var parameter = Expression.Parameter(typeof(MondayMorningBlues), "p");
        //        var propertyReference = Expression.Property(parameter, "Email");
        //        //var propertyReference = properties.FirstOrDefault((n) => n.Name == "Email");
        //        var result =  Expression.Lambda<Func<MondayMorningBlues, object>>
        //                (propertyReference, new[] { parameter }).Compile()(data[0]);

        //        // Compose the expression tree that represents the parameter to the predicate.
        //        ParameterExpression pe = Expression.Parameter(typeof(MondayMorningBlues), "Email");

        //        // ***** Where(company => (company.ToLower() == "coho winery" || company.Length > 16)) *****
        //        // Create an expression tree that represents the expression 'company.ToLower() == "coho winery"'.
        //        Expression left = Expression.Call(pe, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
        //        Expression right = Expression.Constant("pstancer+4@gmail.com");
        //        Expression e1 = Expression.Equal(left, right);


        //        // Create an expression tree that represents the expression
        //        // 'queryableData.Where(company => (company.ToLower() == "coho winery" || company.Length > 16))'
        //        MethodCallExpression whereCallExpression = Expression.Call(
        //            typeof(Queryable),
        //            "Where",
        //            new Type[] { queryableData.ElementType },
        //            queryableData.Expression,
        //            Expression.Lambda<Func<string, bool>>(e1, new ParameterExpression[] { pe }));
        //        // ***** End Where *****

        //        // ***** OrderBy(company => company) *****
        //        // Create an expression tree that represents the expression
        //        // 'whereCallExpression.OrderBy(company => company)'
        //        MethodCallExpression orderByCallExpression = Expression.Call(
        //            typeof(Queryable),
        //            "OrderBy",
        //            new Type[] { queryableData.ElementType, queryableData.ElementType },
        //            whereCallExpression,
        //            Expression.Lambda<Func<string, string>>(pe, new ParameterExpression[] { pe }));
        //        // ***** End OrderBy *****

        //        // Create an executable query from the expression tree.
        //        var results = queryableData.Provider.CreateQuery<MondayMorningBlues>(whereCallExpression);
        //        //var results = queryableData.Provider.CreateQuery<MondayMorningBlues>(orderByCallExpression);

        //        var list = results.ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}
    }
}
