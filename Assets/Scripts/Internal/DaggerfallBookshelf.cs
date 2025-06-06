// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Bookshelves in building interiors which allow reading books.
    /// </summary>
    public class DaggerfallBookshelf : MonoBehaviour
    {
        private List<int> books = null;

        void Start()
        {
            if (books == null)
            {
                // Populate bookshelf with up to 10 random books.
                books = new List<int>();
                for (int i=0; i<10; i++)
                {
                    int bookNum = DaggerfallUnity.Instance.ItemHelper.GetRandomBookID();
                    string bookName = DaggerfallUnity.Instance.ItemHelper.GetBookTitle(bookNum, string.Empty);
                    if (bookName != string.Empty)
                        books.Add(bookNum);
                }
            }
        }

        // Subclass DaggerfallListPickerWindow to delay calls to listBox.AddItem() after its setup
        // Otherwise, added items do not inherit the restricted area from the ListBox
        private class DaggerfallBooksListPickerWindow : DaggerfallListPickerWindow
        {
            private List<int> books;

            public DaggerfallBooksListPickerWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previous, List<int> books) : base(uiManager, previous)
            {
                this.books = books;
            }

            protected override void Setup()
            {
                base.Setup();
                listBox.RectRestrictedRenderArea = new Rect(listBox.Position, listBox.Size);
                listBox.RestrictedRenderAreaCoordinateType = BaseScreenComponent.RestrictedRenderArea_CoordinateType.ParentCoordinates;
                foreach (int bookNum in books)
                {
                    string bookName = DaggerfallUnity.Instance.ItemHelper.GetBookTitle(bookNum, string.Empty);
                    listBox.AddItem(bookName);
                }
            }
        }

        public void ReadBook()
        {
            // Check permission to access bookshelf if inside a guild or temple
            PlayerGPS.DiscoveredBuilding buildingData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
            int factionID = buildingData.factionID;
            Debug.Log("Bookshelf access, Faction ID = " + factionID);
            IGuild guild = GameManager.Instance.GuildManager.GetGuild(factionID);
            if ((buildingData.buildingType == DFLocation.BuildingTypes.GuildHall ||
                 buildingData.buildingType == DFLocation.BuildingTypes.Temple) &&
                 !guild.CanAccessLibrary())
            {
                DaggerfallUI.MessageBox(TextManager.Instance.GetLocalizedText("accessMembersOnly"));
            }
            else
            {
                // Show book picker loaded with list of books on this shelf
                IUserInterfaceManager uiManager = DaggerfallUI.UIManager;
                DaggerfallListPickerWindow bookPicker = new DaggerfallBooksListPickerWindow(uiManager, uiManager.TopWindow, books);
                bookPicker.OnItemPicked += BookShelf_OnItemPicked;
                uiManager.PushWindow(bookPicker);
            }
        }

        public void BookShelf_OnItemPicked(int index, string bookName)
        {
            DaggerfallUI.Instance.BookReaderWindow.OpenBook(books[index]);
            DaggerfallUI.UIManager.PopWindow();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenBookReaderWindow);
        }
    }
}