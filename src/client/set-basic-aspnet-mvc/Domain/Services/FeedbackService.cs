using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using set_basic_aspnet_mvc.Domain.Entities;
using set_basic_aspnet_mvc.Domain.Repositories;
using set_basic_aspnet_mvc.Helpers;

namespace set_basic_aspnet_mvc.Domain.Services
{
    public interface IFeedbackService
    {
        Task<bool> AddFeedback(long userId, string userEmail, string info);
        
        Task<Feedback> GetFeedback(int id);
        
        Task<bool> SetFeedbackToReviewed(int id);

        Task<PagedList<Feedback>> GetFeedbacks(int lastId, int page, bool isReviewed = false);
    }

    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<Feedback> _feedbackRepo;

        public FeedbackService(IRepository<Feedback> feedbackRepo)
        {
            _feedbackRepo = feedbackRepo;
        }
        
        public Task<bool> AddFeedback(long userId, string userEmail, string info)
        {
            if (string.IsNullOrEmpty(info) || 
                string.IsNullOrEmpty(userEmail) || 
                !userEmail.IsEmail()) 
                return Task.FromResult(false);

            var newFeedback = new Feedback
            {
                UserId = userId,
                UserEmail = userEmail,
                Info = info
            };
            
            _feedbackRepo.Create(newFeedback);

            var result = _feedbackRepo.SaveChanges();

            return Task.FromResult(result);
        }

        public Task<Feedback> GetFeedback(int id)
        {
            if (id < 0) return null;

            var feedback = _feedbackRepo.FindOne(x => x.Id == id);
            return Task.FromResult(feedback);
        }
        
        public async Task<bool> SetFeedbackToReviewed(int id)
        {
            if (id < 0) return await Task.FromResult(false);

            var feedback = await GetFeedback(id);
            if (feedback == null) return await Task.FromResult(false);

            feedback.IsReviewed = true;
            feedback.ReviewedAt = DateTime.Now;

            _feedbackRepo.Update(feedback);

            var result = _feedbackRepo.SaveChanges();

            return await Task.FromResult(result);
        }

        public Task<PagedList<Feedback>> GetFeedbacks(int lastId, int page, bool isReviewed = false)
        {
            _feedbackRepo.FindAll(x => x.Id > lastId);

            var items = page < 1 ? _feedbackRepo.FindAll()
                                 : _feedbackRepo.FindAll(x => x.Id > lastId);

            items = items.Where(x => x.IsReviewed == isReviewed);

            long totalCount = items.Count();
            items = items.OrderByDescending(x => x.Id).Skip(ConstHelper.PageSize * page).Take(ConstHelper.PageSize);

            return Task.FromResult(new PagedList<Feedback>(page, ConstHelper.PageSize, totalCount, items));
        }
    }
}