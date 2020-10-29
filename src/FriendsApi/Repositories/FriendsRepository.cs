using FootballContacts.Contexts;
using FriendsApi.Model.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace FootballContacts.Repositories
{

    public interface IFriendsRepository
	{
		Friend Get(int id);
		List<Friend> List();
		Friend Create(Friend friend);
		bool Update(Friend friend);
		bool Delete(int id);
	}

	public class FriendsRepository : IFriendsRepository
	{
		private readonly FriendContext _dbContext;
		private readonly ILogger<FriendsRepository> _logger;

		public FriendsRepository(FriendContext dbContext, ILogger<FriendsRepository> logger)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		public Friend Get(int id)
        {
			return _dbContext.Friends.Where(x => x.Id == id).FirstOrDefault();
		}
		public List<Friend> List()
		{
			return _dbContext.Friends.ToList();
		}

		public Friend Create(Friend friend)
		{
			_dbContext.Friends.Add(friend);

			_dbContext.SaveChanges();

			_logger.LogInformation("Friend Contact created - dto: {0}", JsonSerializer.Serialize(friend));
			return friend;
		}

		public bool Update(Friend friend)
		{
			var record = _dbContext.Friends
				.AsNoTracking().FirstOrDefault(x => x.Id == friend.Id);

			if (record != null)
			{
				_dbContext.Friends.Update(friend);

				_dbContext.SaveChanges();

				_logger.LogInformation("Friend Contact updated - dto: {0}", JsonSerializer.Serialize(friend));


				return true;
			}

			_logger.LogInformation("Friend Contact insert failed - dto: {0}", JsonSerializer.Serialize(friend));


			return false;
		}

		public bool Delete(int id)
		{
			var record = _dbContext.Friends
				.AsNoTracking().FirstOrDefault(x => x.Id == id);

			if (record != null)
			{
				_dbContext.Friends.Remove(record);

				_dbContext.SaveChanges();

				_logger.LogInformation("Friend Contact deleted - dto: {0}", JsonSerializer.Serialize(record));


				return true;
			}

			_logger.LogInformation("Friend Contact delete failed: {0}", id);

			return false;

		}

	}
}
