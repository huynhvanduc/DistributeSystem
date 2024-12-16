﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Contract.Abstractions.Shared
{
    public class PagedResult<T>
    {
        public const int UpdatePageSize = 100;
        public const int DefaultPageSize = 10;
        public const int DefaultPageIndex = 1;

        public List<T> Items { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public bool HasNextPage => PageIndex * PageSize < TotalCount;

        public bool HasPrivousPage => PageIndex > 1;

        private PagedResult(List<T> items, int pageIndex, int pageSize, int totalCount)
        {
            Items = items;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        private static async Task<PagedResult<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex <= 0 ? DefaultPageIndex : pageIndex;
            pageSize = pageSize <= 0 ?
                DefaultPageSize :
                pageSize > UpdatePageSize
                ? UpdatePageSize : pageSize;

            var totalCount = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new(items, pageIndex, pageSize, totalCount);
        }

        public static PagedResult<T> Create(List<T> items, int pageIndex, int pageSize, int totalCount)
        => new(items, pageIndex, pageSize, totalCount);
    }
}
