﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Xunit;

namespace DataUnitTest
{
    public class WrappedTest
    {
        [Fact]
        public async Task AddAndRemoveTest()
        {
            UnitTestEnvironment env = new UnitTestEnvironment();
            using (var scope = env.GetServiceScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var database = serviceProvider.GetService<IDatabaseContext>();
                var categoryContext = database.GetWrappedContext<ICategory>();
                var userContext = database.GetWrappedContext<IUser>();
                Assert.Equal(0, await categoryContext.CountAsync(categoryContext.Query()));
                var category = categoryContext.Create();
                category.Name = "Test";
                categoryContext.Add(category);
                await database.SaveAsync();

                var user = userContext.Create();
                user.Username = "TestUser";
                user.Category = category;
                userContext.Add(user);
                await database.SaveAsync();

                Assert.Equal(1, await userContext.CountAsync(userContext.Query().Where(t => t.Category.Name == "Test")));

                user = await userContext.GetAsync(user.Index);
                Assert.NotNull(user);
            }
        }
    }
}
