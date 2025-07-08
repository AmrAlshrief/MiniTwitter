using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Events;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Domain.Events;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    public class FollowService : IFollowService
    {
        private readonly IGenericService<Follow> _followRepo;
        private readonly IUserRepository _userRepo;
        private readonly IEventDispatcher _eventDispatcher;

        public FollowService(IGenericService<Follow> followRepo, IUserRepository userRepo, IEventDispatcher eventDispatcher)
        {
            _followRepo = followRepo;
            _userRepo = userRepo;
            _eventDispatcher = eventDispatcher;
        }


        public async Task FollowUserAsync(int followerId, int followingId)
        {

            if (followerId == followingId)
                throw new InvalidOperationException("You cannot follow yourself.");

            var alreadyFollowing = await _followRepo.ExistsAsync(f =>
                f.FollowerId == followerId && f.FollowingId == followingId);

            if (alreadyFollowing)
                throw new InvalidOperationException("You are already following this user.");

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,
                FollowedAt = DateTime.UtcNow
            };

            // await _followRepo.AddAsync(follow);

            // await _eventDispatcher.DispatchAsync(new UserFollowedEvent(followerId, followingId));

            await _followRepo.AddAsync(follow);
            // Dispatch the event after adding the follow
            await _eventDispatcher.DispatchAsync(new UserFollowedEvent(followerId, followingId));
        }

        public async Task<IEnumerable<UserDto>> GetFollowersAsync(int userId)
        {
            var follows = await _followRepo.FindAllAsync(
            f => f.FollowingId == userId,
            f => f.Follower // Include the related User (Following)
        );

            return follows.Select(f => new UserDto
            {
                Id = f.Follower.Id,
                UserName = f.Follower.Username,
                Email = f.Follower.Email,
            });
        }

        public async Task<IEnumerable<UserDto>> GetFollowingAsync(int userId)
        {
            var follows = await _followRepo.FindAllAsync(
                f => f.FollowerId == userId,
                f => f.Following
            );


            return follows.Select(f => new UserDto
            {
                Id = f.Following.Id,
                UserName = f.Following.Username,
                Email = f.Following.Email
            });
        }

        public async Task<bool> IsFollowingAsync(int followerId, int followingId)
        {
            return await  _followRepo.ExistsAsync(f =>
                f.FollowerId == followerId && f.FollowingId == followingId);
        }

        public async Task UnfollowUserAsync(int followerId, int followingId)
        {
            var follow = await _followRepo.FindOneAsync(f =>
                f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null)
                throw new KeyNotFoundException("You are not following this user.");

            await _followRepo.DeleteAsync(follow);
        }


    }
}
