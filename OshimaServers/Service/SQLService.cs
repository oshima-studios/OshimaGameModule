using System.Data;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;
using ProjectRedbud.FunGame.SQLQueryExtension;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class SQLService
    {
        public static Offer? GetOffer(SQLHelper helper, long offerId)
        {
            DataRow? dr = helper.ExecuteDataRow(OffersQuery.Select_OfferById(helper, offerId));
            if (dr != null)
            {
                Offer offer = new();
                SetValue(dr, offer);
                return offer;
            }
            return null;
        }

        public static List<Offer> GetOffersByOfferor(SQLHelper helper, long offerorId)
        {
            List<Offer> offers = [];
            DataSet ds = helper.ExecuteDataSet(OffersQuery.Select_OffersByOfferor(helper, offerorId));
            if (helper.Success)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Offer offer = new();
                    SetValue(dr, offer);
                    offers.Add(offer);
                }
            }
            return offers;
        }

        public static List<Offer> GetOffersByOfferee(SQLHelper helper, long offereeId)
        {
            List<Offer> offers = [];
            DataSet ds = helper.ExecuteDataSet(OffersQuery.Select_OffersByOfferee(helper, offereeId));
            if (helper.Success)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Offer offer = new();
                    SetValue(dr, offer);
                    offers.Add(offer);
                }
            }
            return offers;
        }

        public static List<Guid> GetOfferItemsByOfferIdAndUserId(SQLHelper helper, long offerId, User user, long userId = -1)
        {
            if (userId == -1) userId = user.Id;
            List<Guid> itemGuids = [];
            DataSet ds = helper.ExecuteDataSet(OfferItemsQuery.Select_OfferItemsByOfferIdAndUserId(helper, offerId, userId));
            if (user != null && helper.Success)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (user.Inventory.Items.FirstOrDefault(i => i.Guid.ToString().EqualsGuid(dr[OfferItemsQuery.Column_ItemGuid])) is Item item)
                    {
                        itemGuids.Add(item.Guid);
                    }
                }
            }
            return itemGuids;
        }
        
        public static List<Guid> GetUserItemGuids(SQLHelper helper, long userId)
        {
            helper.Parameters["UserId"] = userId;
            DataSet ds = helper.ExecuteDataSet($"select {OfferItemsQuery.TableName}.{OfferItemsQuery.Column_ItemGuid} from {OffersQuery.TableName}\r\n" + 
                $"left join {OfferItemsQuery.TableName} on {OfferItemsQuery.TableName}.{OfferItemsQuery.Column_OfferId} = {OffersQuery.TableName}.{OffersQuery.Column_Id}\r\n" +
                $"where {OfferItemsQuery.Column_UserId} = @UserId and {OffersQuery.Column_Status} not in ({(int)OfferState.Cancelled}, {(int)OfferState.Rejected}, {(int)OfferState.Completed})");
            List<Guid> list = [];
            if (helper.Success)
            {
                list = [.. ds.Tables[0].AsEnumerable().Select(dr => Guid.TryParse(dr.Field<string>(OfferItemsQuery.Column_ItemGuid), out Guid guid) ? guid : Guid.Empty)];
            }
            return list;
        }
        
        public static bool IsItemInOffers(SQLHelper helper, Guid itemGuid)
        {
            helper.Parameters["ItemGuid"] = itemGuid.ToString();
            helper.ExecuteDataSet($"select {OffersQuery.TableName}.{OffersQuery.Column_Id} from {OffersQuery.TableName}\r\n" + 
                $"left join {OfferItemsQuery.TableName} on {OfferItemsQuery.TableName}.{OfferItemsQuery.Column_OfferId} = {OffersQuery.TableName}.{OffersQuery.Column_Id}\r\n" +
                $"where {OfferItemsQuery.Column_ItemGuid} = @ItemGuid and {OffersQuery.Column_Status} not in ({(int)OfferState.Cancelled}, {(int)OfferState.Rejected}, {(int)OfferState.Completed})");
            return helper.Success;
        }

        public static void AddOffer(SQLHelper helper, long offeror, long offeree, OfferState status = OfferState.Created, int negotiatedTimes = 0)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                helper.Execute(OffersQuery.Insert_Offer(helper, offeror, offeree, status, negotiatedTimes));
                if (!helper.Success) throw new Exception($"新增报价 (Offeror: {offeror}, Offeree: {offeree}) 失败。");

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }

        public static void AddOfferItem(SQLHelper helper, long offerId, long userId, Guid itemGuid)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                helper.Execute(OfferItemsQuery.Insert_OfferItem(helper, offerId, userId, itemGuid));
                if (!helper.Success) throw new Exception($"新增报价物品 (OfferId: {offerId}, UserId: {userId}, ItemGuid: {itemGuid}) 失败。");

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }

        public static void UpdateOfferStatus(SQLHelper helper, long id, OfferState status)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                helper.Execute(OffersQuery.Update_OfferStatus(helper, id, status));
                if (!helper.Success) throw new Exception($"更新报价 {id} 状态失败。");

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }

        public static void UpdateOfferNegotiatedTimes(SQLHelper helper, long id, int negotiatedTimes)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                helper.Execute(OffersQuery.Update_OfferNegotiatedTimes(helper, id, negotiatedTimes));
                if (!helper.Success) throw new Exception($"更新报价 {id} 协商次数失败。");

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }

        public static void UpdateOfferFinishTime(SQLHelper helper, long id, DateTime finishTime)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                helper.Execute(OffersQuery.Update_OfferFinishTime(helper, id, finishTime));
                if (!helper.Success) throw new Exception($"更新报价 {id} 完成时间失败。");

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }

        public static void DeleteOffer(SQLHelper helper, long id)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                // 删除 Offer 相关的 OfferItems
                helper.DeleteOfferItemsByOfferId(id);
                helper.Execute(OffersQuery.Delete_Offer(helper, id));

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }

        public static void DeleteOfferItemsByOfferId(SQLHelper helper, long offerId)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                helper.Execute(OfferItemsQuery.Delete_OfferItemsByOfferId(helper, offerId));

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }
        
        public static void DeleteOfferItemsByOfferIdAndItemGuid(SQLHelper helper, long offerId, Guid itemGuid)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                helper.Execute(OfferItemsQuery.Delete_OfferItemsByOfferIdAndItemGuid(helper, offerId, itemGuid));

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }

        public static void DeleteOfferItem(SQLHelper helper, long id)
        {
            bool hasTransaction = helper.Transaction != null;
            if (!hasTransaction) helper.NewTransaction();

            try
            {
                helper.Execute(OfferItemsQuery.Delete_OfferItem(helper, id));

                if (!hasTransaction) helper.Commit();
            }
            catch (Exception)
            {
                if (!hasTransaction) helper.Rollback();
                throw;
            }
        }

        private static void SetValue(DataRow dr, Offer offer)
        {
            offer.Id = (long)dr[OffersQuery.Column_Id];
            offer.Offeror = (long)dr[OffersQuery.Column_Offeror];
            offer.Offeree = (long)dr[OffersQuery.Column_Offeree];
            offer.Status = (OfferState)Convert.ToInt32(dr[OffersQuery.Column_Status]);
            offer.NegotiatedTimes = Convert.ToInt32(dr[OffersQuery.Column_NegotiatedTimes]);

            if (dr[OffersQuery.Column_CreateTime] != DBNull.Value && DateTime.TryParse(dr[OffersQuery.Column_CreateTime].ToString(), out DateTime dt))
            {
                offer.CreateTime = dt;
            }

            if (dr[OffersQuery.Column_FinishTime] != DBNull.Value && DateTime.TryParse(dr[OffersQuery.Column_FinishTime].ToString(), out dt))
            {
                offer.FinishTime = dt;
            }
        }
    }
}
