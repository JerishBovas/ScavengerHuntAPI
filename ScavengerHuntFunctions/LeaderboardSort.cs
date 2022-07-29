using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ScavengerHuntFunctions.Models;

namespace ScavengerHuntFunctions
{
    public class LeaderboardSort
    {
        [FunctionName("LeaderboardSort")]
        public async Task Run(
            [TimerTrigger("0 * * * * *")]TimerInfo myTimer, 
            [CosmosDB("ScavengerHuntAPI", "Users",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT u.Id, u.Email, u.UserLog FROM Users u"
                )] IEnumerable<User> users,
            [CosmosDB(databaseName: "ScavengerHuntAPI",
                collectionName: "Leaderboard",
                ConnectionStringSetting = "CosmosDBConnection",
                CreateIfNotExists = true,
                PartitionKey = "/Id",
                CollectionThroughput = 400)] dynamic leaderboard   
                , ILogger log)
        {
            List<User> userList = users.ToList();
            userList.Sort(delegate(User x, User y) {
                return x.UserLog.UserScore.CompareTo(y.UserLog.UserScore);
            });

            await leaderboard.FlushAsync();
            foreach(User user in userList){
                await leaderboard.AddAsync(user);
                log.LogInformation(user.Id.ToString());
                log.LogInformation(user.UserLog.UserScore.ToString());
            }
        }
    }
}

