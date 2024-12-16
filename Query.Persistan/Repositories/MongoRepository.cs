﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Query.Domain.Abstractions;
using Query.Domain.Abstractions.Options;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Attributes;
using System.Linq.Expressions;

namespace Query.Persistence.Repositories;

public class MongoRepository<TDocument> : IMongoRepository<TDocument>
    where TDocument : IDocument
{
    private readonly IMongoCollection<TDocument> _collection;
    public MongoRepository(IMongoDbSettings settings)
    {
        var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
        _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
    }

    private protected string GetCollectionName(Type documentType)
    {
        return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
            typeof(BsonCollectionAttribute),
            true)
            .FirstOrDefault())?.CollectionName;
    }

    public virtual void DeleteById(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        _collection.FindOneAndDelete(filter);
    }

    public virtual Task DeleteByIdAsync(string id)
    {
        return Task.Run(() =>
        {
            var objectid = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectid);
            _collection.FindOneAndDeleteAsync(filter);
        });
    }

    public virtual void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
    {
        _collection.DeleteMany(filterExpression);
    }

    public virtual Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() =>
        {
            _collection.DeleteMany(filterExpression);
        });
    }

    public virtual void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        _collection.FindOneAndDelete(filterExpression);
    }

    public virtual Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() =>
        {
            _collection.FindOneAndDelete(filterExpression);
        });
    }

    public virtual IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).ToEnumerable();
    }

    public virtual IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<TDocument, bool>> filterExpression, Expression<Func<TDocument, TProjected>> projectionExpression)
    {
        return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
    }

    public virtual TDocument FindById(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        return _collection.Find(filter).SingleOrDefault();
    }

    public virtual Task<TDocument> FindByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).FirstOrDefault();
    }

    public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
    }

    public virtual void InsertMany(ICollection<TDocument> documents)
    {
        _collection.InsertMany(documents);
    }

    public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
    {
        await _collection.InsertManyAsync(documents);
    }

    public virtual void InsertOne(TDocument document)
    {
        _collection.InsertOne(document);
    }

    public virtual Task InsertOneAsync(TDocument document)
    {
        return Task.Run(() => _collection.InsertOneAsync(document));
    }

    public virtual void ReplaceOne(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        _collection.FindOneAndReplace(filter, document);
    }

    public virtual async Task ReplaceOneAsync(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        await _collection.FindOneAndReplaceAsync(filter, document);
    }

    public IMongoQueryable<TDocument> AsQueryable(Expression<Func<TDocument, bool>>? filterExpression = null)
    {
        if (filterExpression is not null)
        {
            return _collection.AsQueryable().Where(filterExpression);
        }

        return _collection.AsQueryable();
    }
}