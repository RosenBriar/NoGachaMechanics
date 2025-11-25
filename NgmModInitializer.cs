using System.Collections.Generic;

namespace NoGachaMechanics
{
    public class NgmModInitializer : ModInitializer
    {
        public static string PackageId = "viviandusk.nogachamechanics";

        private static class RandomGachaGenerator
        {
            public static BookDropItemInfo SelectRandomItem(DropBookXmlInfo book)
            {
                var dropBoxCountList1 = new List<DropBoxCount>();
                var dropBoxCountList2 = new List<DropBoxCount>();
                var dropBoxCountList3 = new List<DropBoxCount>();
                var dropBoxCountList4 = new List<DropBoxCount>();
                var dropBoxCountList5 = new List<DropBoxCount>();
                var dropBoxCountList6 = new List<DropBoxCount>();
                var dropBoxCountList7 = new List<DropBoxCount>();
                var dropBoxCountList8 = new List<DropBoxCount>();
                var boxCountInfoTable = Singleton<DropBoxListModel>.Instance.GetEquipDropBoxCountInfoTable(book.id);
                foreach (var dropBoxCount in boxCountInfoTable.FindAll(x => x.itemInfo.itemType == DropItemType.Equip))
                {
                    var data = Singleton<BookXmlList>.Instance.GetData(dropBoxCount.itemInfo.id);
                    if (data != null && dropBoxCount.remain > 0)
                        switch (data.Rarity)
                        {
                            case Rarity.Common:
                                dropBoxCountList1.Add(dropBoxCount);
                                continue;
                            case Rarity.Uncommon:
                                dropBoxCountList2.Add(dropBoxCount);
                                continue;
                            case Rarity.Rare:
                                dropBoxCountList3.Add(dropBoxCount);
                                continue;
                            case Rarity.Unique:
                                dropBoxCountList4.Add(dropBoxCount);
                                continue;
                            default:
                                continue;
                        }
                }

                foreach (var dropBoxCount in boxCountInfoTable.FindAll(x => x.itemInfo.itemType == DropItemType.Card))
                    if (dropBoxCount.remain > 0)
                    {
                        var cardItem = ItemXmlDataList.instance.GetCardItem(dropBoxCount.itemInfo.id, true);
                        if (cardItem != null)
                            switch (cardItem.Rarity)
                            {
                                case Rarity.Common:
                                    dropBoxCountList5.Add(dropBoxCount);
                                    continue;
                                case Rarity.Uncommon:
                                    dropBoxCountList6.Add(dropBoxCount);
                                    continue;
                                case Rarity.Rare:
                                    dropBoxCountList7.Add(dropBoxCount);
                                    continue;
                                case Rarity.Unique:
                                    dropBoxCountList8.Add(dropBoxCount);
                                    continue;
                                default:
                                    continue;
                            }
                    }
                var probInfoList1 = new List<ProbInfo>();
                var probInfoList2 = new List<ProbInfo>();
                var probInfoList3 = new List<ProbInfo>();
                probInfoList2.Add(new ProbInfo
                {
                    type = DropItemType.Equip,
                    dropList = dropBoxCountList1
                });
                probInfoList2.Add(new ProbInfo
                {
                    type = DropItemType.Equip,
                    dropList = dropBoxCountList2
                });
                probInfoList2.Add(new ProbInfo
                {
                    type = DropItemType.Equip,
                    dropList = dropBoxCountList3
                });
                probInfoList2.Add(new ProbInfo
                {
                    type = DropItemType.Equip,
                    dropList = dropBoxCountList4
                });
                probInfoList3.Add(new ProbInfo
                {
                    type = DropItemType.Card,
                    dropList = dropBoxCountList5
                });
                probInfoList3.Add(new ProbInfo
                {
                    type = DropItemType.Card,
                    dropList = dropBoxCountList6
                });
                probInfoList3.Add(new ProbInfo
                {
                    type = DropItemType.Card,
                    dropList = dropBoxCountList7
                });
                probInfoList3.Add(new ProbInfo
                {
                    type = DropItemType.Card,
                    dropList = dropBoxCountList8
                });
                for (var index = 0; index < 4; ++index)
                    if (probInfoList2[index].dropList.Count == 0)
                        probInfoList3[index].prob += probInfoList2[index].prob;
                RestructureProb(probInfoList2);
                RestructureProb(probInfoList3);
                probInfoList1.AddRange(probInfoList2);
                probInfoList1.AddRange(probInfoList3);
                if (probInfoList1.Count <= 0)
                    return null;
                var num10 = 0.0;
                foreach (var probInfo in probInfoList1)
                    num10 += probInfo.prob;
                if (num10 <= 0.0)
                    return null;
                var dropBoxCount1 = (DropBoxCount)null;
                for (var index = 0; index < 1; ++index)
                {
                    if (LibraryModel.Instance.PlayHistory.Tutorial_GetFirstCoreBook == 0 && book.id == 200001)
                    {
                        LibraryModel.Instance.PlayHistory.Tutorial_GetFirstCoreBook = 1;
                        var dropBoxCount2 =
                            boxCountInfoTable.Find(x =>
                                x.itemInfo.itemType == DropItemType.Equip);
                        if (dropBoxCount2 != null)
                        {
                            dropBoxCount1 = dropBoxCount2;
                            continue;
                        }
                    }

                    var num11 = RandomUtil.valueForProb * num10;
                    var dropBoxCount3 = (DropBoxCount)null;
                    var num12 = 0.0;
                    foreach (var probInfo in probInfoList1)
                    {
                        num12 += probInfo.prob;
                        if (num11 < num12)
                        {
                            dropBoxCount3 = RandomUtil.SelectOne(probInfo.dropList);
                            break;
                        }
                    }

                    if (dropBoxCount3 != null)
                        dropBoxCount1 = dropBoxCount3;
                }

                return dropBoxCount1?.itemInfo;
            }

            private static void RestructureProb(List<ProbInfo> probInfoList)
            {
                probInfoList.RemoveAll(x => x.dropList.Count == 0);
            }

            private class ProbInfo
            {
                public List<DropBoxCount> dropList;
                public float prob = 1;
                public DropItemType type;
            }
        }


        public class NoRandomGachaGenerator : LibraryFloorModel
        {
            private SephirahType _sephirah;

            private bool ExistsUnique(DropBookXmlInfo dropBook)
            {
                foreach (var dropItem in dropBook.DropItemList)
                    if (dropItem.itemType == DropItemType.Card)
                    {
                        var cardItem = ItemXmlDataList.instance.GetCardItem(dropItem.id);
                        if (cardItem != null && cardItem.Rarity == Rarity.Unique)
                            return true;
                    }
                    else if (dropItem.itemType == DropItemType.Equip)
                    {
                        var data = Singleton<BookXmlList>.Instance.GetData(dropItem.id);
                        if (data != null && data.Rarity == Rarity.Unique)
                            return true;
                    }

                return false;
            }

            public new List<BookDropResult> FeedBook(DropBookXmlInfo book)
            {
                var bookDropResultList = new List<BookDropResult>();
                for (var index = 0; index < book.DropNum; ++index)
                {
                    var bookDropItemInfo = RandomGachaGenerator.SelectRandomItem(book);
                    if (bookDropItemInfo != null)
                    {
                        var bookDropResult = new BookDropResult
                        {
                            id = bookDropItemInfo.id,
                            itemType = bookDropItemInfo.itemType
                        };
                        if (bookDropResult.itemType == DropItemType.Card)
                        {
                            Singleton<DropBoxListModel>.Instance.AddCardCount(book.id, bookDropResult.id);
                            var cardItem = ItemXmlDataList.instance.GetCardItem(bookDropResult.id);
                            if (cardItem != null)
                            {
                                var limit = cardItem.Limit;
                                var num = 0 + Singleton<InventoryModel>.Instance.GetCardCount(bookDropResult.id);
                                foreach (var openedFloor in LibraryModel.Instance.GetOpenedFloorList())
                                    num += openedFloor.GetEquipedCardCount(bookDropResult.id);
                                if (num >= 150)
                                    bookDropResult.hasLimit = true;
                                else
                                    Singleton<InventoryModel>.Instance.AddCard(bookDropResult.id);
                            }
                        }
                        else if (bookDropResult.itemType == DropItemType.Equip)
                        {
                            Singleton<DropBoxListModel>.Instance.AddEquipCount(book.id, bookDropResult.id);
                            var book1 = Singleton<BookInventoryModel>.Instance.CreateBook(bookDropResult.id);
                            bookDropResult.bookInstanceId = book1.instanceId;
                        }

                        bookDropResultList.Add(bookDropResult);
                    }

                    if (Singleton<DropBoxListModel>.Instance.IsEmptyBox(book.id))
                    {
                        if (ExistsUnique(book))
                            PlatformManager.Instance.UnlockAchievement(AchievementEnum.BURN_ONETYPE);
                        Singleton<DropBoxListModel>.Instance.Reset(book.id);
                    }
                }

                Singleton<LibraryQuestManager>.Instance.OnUseBook(_sephirah, book.id);
                Singleton<DropBookInventoryModel>.Instance.RemoveBook(book.id);
                PlatformManager.Instance.UnlockAchievement(AchievementEnum.BURN_1);
                return bookDropResultList;
            }
        }
    }
}