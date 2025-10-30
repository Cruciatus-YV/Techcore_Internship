using MongoDB.Driver;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Mongo;

public class ProductReviewRepository : IProductReviewRepository
{
    private readonly IMongoCollection<ProductReviewEntity> _reviews;

    public ProductReviewRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Techcore_Internship_Mongo");
        _reviews = database.GetCollection<ProductReviewEntity>("reviews");
    }
}
