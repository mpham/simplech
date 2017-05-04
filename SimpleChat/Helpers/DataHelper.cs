﻿using SimpleChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleChat.Helpers
{
    public class DataHelper
    {
        public static object FindUsersByChat(int chatId)
        {
            SimpleChatContext db = new SimpleChatContext();
            var userChats = db.UserChats
                .Include("User")
                .Where(uc => uc.ChatId == chatId)
                .Select(uc => new { id = uc.User.UserId, name = uc.User.Name, email = uc.User.Email });

            return userChats;
        }

        public static object FindLastMessageByChat(int chatId)
        {
            SimpleChatContext db = new SimpleChatContext();
            ChatMessage cm = db.ChatMessages
                .Include("User")
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            //return new
            //{
            //    id = cm.ChatMessageId,
            //    chat_id = cm.ChatId,
            //    user_id = cm.UserId,
            //    message = cm.Message,
            //    created_at = cm.CreatedAt,
            //    user = new
            //    {
            //        id = cm.User.UserId,
            //        name = cm.User.Name,
            //        email = cm.User.Email
            //    }
            //};
            return BuildChatMessageResponseData(cm, cm.User);
        }

        public static int CalculatePageCount(int limit, int total)
        {
            int pageCount = total / limit;
            if ((total % limit) > 0)
            {
                pageCount++;
            }
            return pageCount;
        }

        public static object BuildChatMessageResponseData(ChatMessage chatmessage, User user)
        {
            return new
            {
                id = chatmessage.ChatMessageId,
                chat_id = chatmessage.ChatId,
                user_id = chatmessage.UserId,
                message = chatmessage.Message,
                created_at = chatmessage.CreatedAt,
                user = new
                {
                    id = user.UserId,
                    name = user.Name,
                    email = user.Email
                }
            };
        }
    }
}