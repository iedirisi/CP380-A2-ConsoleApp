﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using RatingAdjustment.Services;
using BreadmakerReport.Models;

namespace BreadmakerReport
{
    class Program
    {
        static string dbfile = @".\data\breadmakers.db";
        static RatingAdjustmentService ratingAdjustmentService = new RatingAdjustmentService();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Bread World");
            var BreadmakerDb = new BreadMakerSqliteContext(dbfile);
            var BMListTemp = BreadmakerDb.Breadmakers
                .Select(row => new
                {
                    Description = row.title,
                    Reviews = row.Reviews.Count(),
                    Average = (Double)BreadmakerDb.Reviews.Where(r => r.BreadmakerId == row.BreadmakerId).Select(r => r.stars).Sum() / row.Reviews.Count(),
                })
                .ToList();

            var BMList = BMListTemp
                .Select(row => new
                {
                    Description = row.Description,
                    Reviews = row.Reviews,
                    Average = row.Average,
                    Adjust = ratingAdjustmentService.Adjust(row.Average, row.Reviews)
                })
                .OrderByDescending(r => r.Adjust)
                .ToList();

            Console.WriteLine("[#]  Reviews Average  Adjust    Description");
            for (var i = 0;  i< 3; i++)
            {
                var j = BMList[i];
                Console.WriteLine( "[{0}] {1} {2} {3} {4}", i+1, j.TotalReviews, j.AverageReviews, j.AdjustReview, j.title);
            }
        }
    }
}
