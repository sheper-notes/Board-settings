﻿using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class BoardQueries : IBoardQueries
    {
        private readonly Database db;

        public BoardQueries(Database database)
        {
            db = database;
        }

        public async Task<Board> GetBoardById(long Id) 
        {
            return await db.Boards.FindAsync(Id);
        }

        public async Task<IEnumerable<Board>> GetBoardsForUser(string Id)
        {
            return await db.Boards.Where(x => x.Users.Any(y => y.UserId == Id)).Include(x => x.Users).ToListAsync();
        }

        public async Task<IEnumerable<Board>> GetOwnedBoardsForUser(string Id)
        {
            return await db.Boards.Where(x => x.Users.Any(y => y.UserId == Id && y.Role == Role.Owner)).ToListAsync();
        }

        public async Task<IEnumerable<Board>> GetBoardsForUser(string Id, int page)
        {
            return await db.Boards.Where(x => x.Users.Any(y => y.UserId == Id)).Skip((page -1) * 6).Take(6).ToListAsync();
        }


        public async Task UpdateBoardName(long Id, string NewName)
        {
            var board = await db.Boards.FindAsync(Id);
            board.Name = NewName;
            await db.SaveChangesAsync();
        }
        public async Task UpdateSubscriptionType(long Id, SubscriptionType subscriptionType)
        {
            var board = await db.Boards.FindAsync(Id);
            board.SubscriptionType = subscriptionType;
            await db.SaveChangesAsync();
        }

        public async Task<bool> CreateBoard(Board board)
        {
            try
            {
                var res = await db.Boards.AddAsync(board);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> Delete(Board board)
        {
            try
            {
                db.Boards.Remove(board);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

    }
}
