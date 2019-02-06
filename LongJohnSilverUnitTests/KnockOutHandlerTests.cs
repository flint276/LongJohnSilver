using System.Collections.Generic;
using Moq;
using LongJohnSilver.Database;
using NUnit.Framework;

namespace LongJohnSilverUnitTests
{
    public class KnockOutHandlerTests
    {
        public Mock<IDatabase> TestDb;

        [Test]
        public void TurnsLeftForPlayerTestWithNoPlayers()
        {
            TestDb = DataBaseMock();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var turnsLeftForPlayer = knockoutTest.TurnsLeftForPlayer(999);

            Assert.AreEqual(3, turnsLeftForPlayer);
        }

        [Test]
        public void TurnsLeftForPlayerTestWithNewPlayer()
        {
            var playerList = new List<KPlayer>
            {
                new KPlayer {LastPlayed = 0, PlayerId = "999", TurnsLeft = 2},
                new KPlayer {LastPlayed = 0, PlayerId = "998", TurnsLeft = 1}
            };

            TestDb = DataBaseMock(playerList: playerList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var turnsLeftForPlayer = knockoutTest.TurnsLeftForPlayer(900);

            Assert.AreEqual(3, turnsLeftForPlayer);
        }

        [Test]
        public void TurnsLeftForPlayerTestWithMultiplePlayers()
        {
            var playerList = new List<KPlayer>
            {
                new KPlayer {LastPlayed = 0, PlayerId = "999", TurnsLeft = 2},
                new KPlayer {LastPlayed = 0, PlayerId = "998", TurnsLeft = 1}
            };

            TestDb = DataBaseMock(playerList: playerList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var turnsLeftForPlayer = knockoutTest.TurnsLeftForPlayer(999);

            Assert.AreEqual(2, turnsLeftForPlayer);
        }

        [Test]
        public void AddNewContenderTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.AddContender("Test", "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.AddNewContender("Test");

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void ChangeKnockoutTitleTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.AddKnockoutTitle("Test", "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.ChangeKnockoutTitle("Test");

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void EmptyDatabaseTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.ResetAllTables("1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.EmptyDatabase();

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void RebuildDatabaseTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.EmptyKnockoutDatabase("1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.RebuildDataBase();

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void NewDayTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.NewDay("1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.NewDay();

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void SetKnockoutToActiveTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.SetKnockoutToActive("1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.SetKnockoutToActive();

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void EndKnockoutTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.SetKnockoutToEnded("1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.EndKnockout();

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void ApplyScoreChangesForVotingRoundTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.ChangeScore("ToAdd", 1, "1234")).Verifiable();
            TestDb.Setup(m => m.ChangeScore("ToSub", -1, "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.ApplyScoreChangesForVotingRound("ToAdd", "ToSub");

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void CreateNewKnockoutTest()
        {
            TestDb = DataBaseMock();
            TestDb.Setup(m => m.EmptyKnockoutDatabase("1234")).Verifiable();
            TestDb.Setup(m => m.CreateNewKnockout("999", "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.CreateNewKnockout(999);

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void PlayerWentLastTimeTestWithNoPlayers()
        {
            TestDb = DataBaseMock();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var wentLastTime = knockoutTest.PlayerWentLastTime(999);

            Assert.AreEqual(false, wentLastTime);
        }

        [Test]
        public void PlayerWentLastTimeTestWithNewPlayer()
        {
            var playerList = new List<KPlayer>
            {
                new KPlayer {LastPlayed = 0, PlayerId = "999", TurnsLeft = 2},
                new KPlayer {LastPlayed = 0, PlayerId = "998", TurnsLeft = 1}
            };

            TestDb = DataBaseMock(playerList: playerList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var wentLastTime = knockoutTest.PlayerWentLastTime(900);

            Assert.AreEqual(false, wentLastTime);
        }

        [Test]
        public void PlayerWentLastTimeTestWithPlayerWhoHas()
        {
            var playerList = new List<KPlayer>
            {
                new KPlayer {LastPlayed = 1, PlayerId = "999", TurnsLeft = 2},
                new KPlayer {LastPlayed = 0, PlayerId = "998", TurnsLeft = 1}
            };

            TestDb = DataBaseMock(playerList: playerList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var wentLastTime = knockoutTest.PlayerWentLastTime(999);

            Assert.AreEqual(true, wentLastTime);
        }

        [Test]
        public void PlayerWentLastTimeTestWithPlayerWhoHasNot()
        {
            var playerList = new List<KPlayer>
            {
                new KPlayer {LastPlayed = 1, PlayerId = "999", TurnsLeft = 2},
                new KPlayer {LastPlayed = 0, PlayerId = "998", TurnsLeft = 1}
            };

            TestDb = DataBaseMock(playerList: playerList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var wentLastTime = knockoutTest.PlayerWentLastTime(998);

            Assert.AreEqual(false, wentLastTime);
        }

        [Test]
        public void CanWriteAnEpitaphTestWithNoKiller()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "inscription pending", Killer = "999", Name = "TestC1", Score = 0},
                new Contender {Epitaph = "inscription done", Killer = "998", Name = "TestC2", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var canWriteAnEpitaph = knockoutTest.CanWriteAnEpitaph(900);

            Assert.IsFalse(canWriteAnEpitaph);
        }

        [Test]
        public void CanWriteAnEpitaphTestWithKillAndNoPendingEpitaph()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "inscription pending", Killer = "999", Name = "TestC1", Score = 0},
                new Contender {Epitaph = "inscription done", Killer = "998", Name = "TestC2", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var canWriteAnEpitaph = knockoutTest.CanWriteAnEpitaph(998);

            Assert.IsFalse(canWriteAnEpitaph);
        }

        [Test]
        public void CanWriteAnEpitaphTestWithKillerAndPendingEpitaph()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "inscription pending", Killer = "999", Name = "TestC1", Score = 0},
                new Contender {Epitaph = "inscription done", Killer = "998", Name = "TestC2", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var canWriteAnEpitaph = knockoutTest.CanWriteAnEpitaph(999);

            Assert.IsTrue(canWriteAnEpitaph);
        }

        [Test]
        public void WriteEpitaphFromUserTest()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "inscription pending", Killer = "999", Name = "TestC1", Score = 0},
                new Contender {Epitaph = "inscription done", Killer = "998", Name = "TestC2", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);
            TestDb.Setup(m => m.SetEpitaphForContender("TestC1", "TestEpitaph", "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.WriteEpitaphFromUser(999, "TestEpitaph");

            Assert.DoesNotThrow(() => TestDb.Verify());
        }

        [Test]
        public void PlayerHasJustKilledTestWithKiller()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "inscription pending", Killer = "999", Name = "TestC1", Score = 0},
                new Contender {Epitaph = "", Killer = "998", Name = "TestC2", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);
            TestDb.Setup(m => m.SetEpitaphForContender("TestC2", "inscription pending", "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var killTest = knockoutTest.PlayerHasJustKilled();

            Assert.DoesNotThrow(() => TestDb.Verify());
            Assert.AreEqual(998, killTest);
        }

        [Test]
        public void PlayerHasJustKilledTestWithNoKiller()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "inscription pending", Killer = "", Name = "TestC1", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);
            TestDb.Setup(m => m.SetEpitaphForContender("TestC1", "inscription pending", "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var killTest = knockoutTest.PlayerHasJustKilled();

            Assert.Throws<MockException>(() => TestDb.Verify());
            Assert.AreEqual(0, killTest);
        }

        [Test]
        public void PlayerHasJustKilledTestWithNoEpitaph()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "", Killer = "", Name = "TestC1", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);
            TestDb.Setup(m => m.SetEpitaphForContender("TestC1", "inscription pending", "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var killTest = knockoutTest.PlayerHasJustKilled();

            Assert.Throws<MockException>(() => TestDb.Verify());
            Assert.AreEqual(0, killTest);
        }

        [Test]
        public void DeleteContenderTestWithNoFind()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "", Killer = "999", Name = "TestC1", Score = 0},
                new Contender {Epitaph = "", Killer = "998", Name = "TestC2", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);
            TestDb.Setup(m => m.RemoveContender(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var contenderDeleted = knockoutTest.DeleteContender("TestC3");

            Assert.Throws<MockException>(() => TestDb.Verify());
            Assert.IsFalse(contenderDeleted);
        }

        [Test]
        public void DeleteContenderTestWithFind()
        {
            var contenderList = new List<Contender>
            {
                new Contender {Epitaph = "", Killer = "999", Name = "TestC1", Score = 0},
                new Contender {Epitaph = "", Killer = "998", Name = "TestC2", Score = 0}
            };

            TestDb = DataBaseMock(contenderList);
            TestDb.Setup(m => m.RemoveContender("TestC2", "1234")).Verifiable();

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            var contenderDeleted = knockoutTest.DeleteContender("TestC2");

            Assert.DoesNotThrow(() => TestDb.Verify());
            Assert.IsTrue(contenderDeleted);
        }

        [Test]
        public void ApplyPlayerTurnChangesForVotingRoundTestNoPlayer()
        {
            var playerList = new List<KPlayer>
            {
                new KPlayer {LastPlayed = 0, PlayerId = "999", TurnsLeft = 2},
                new KPlayer {LastPlayed = 0, PlayerId = "998", TurnsLeft = 1}
            };

            TestDb = DataBaseMock(playerList: playerList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.ApplyPlayerTurnChangesForVotingRound("900");

            TestDb.Verify(m => m.AddPlayerToKnockout("900", "1234"));
            TestDb.Verify(m => m.ResetAllPlayersLastPlayedStatus("1234"));
            TestDb.Verify(m => m.RegisterPlayersTurn("900", "1234"));
        }

        [Test]
        public void ApplyPlayerTurnChangesForVotingRoundTestWithPlayer()
        {
            var playerList = new List<KPlayer>
            {
                new KPlayer {LastPlayed = 0, PlayerId = "999", TurnsLeft = 2},
                new KPlayer {LastPlayed = 0, PlayerId = "998", TurnsLeft = 1}
            };

            TestDb = DataBaseMock(playerList: playerList);

            var knockoutTest = new KnockOutHandler(1234, TestDb.Object);
            knockoutTest.ApplyPlayerTurnChangesForVotingRound("998");

            TestDb.Verify(m => m.AddPlayerToKnockout(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            TestDb.Verify(m => m.ResetAllPlayersLastPlayedStatus("1234"));
            TestDb.Verify(m => m.RegisterPlayersTurn("998", "1234"));
        }


        public Mock<IDatabase> DataBaseMock(List<Contender> contenderList = null, List<Knockout> knockoutList = null,
            List<KPlayer> playerList = null)
        {
            const string testChannel = "1234";
            var returnDb = new Mock<IDatabase>();

            if (contenderList == null)
                returnDb.Setup(m => m.GetAllContenders(testChannel)).Returns(new List<Contender>());
            else
                returnDb.Setup(m => m.GetAllContenders(testChannel)).Returns(contenderList);

            if (knockoutList == null)
                returnDb.Setup(m => m.GetAllKnockouts(testChannel)).Returns(new List<Knockout>());
            else
                returnDb.Setup(m => m.GetAllKnockouts(testChannel)).Returns(knockoutList);

            if (playerList == null)
                returnDb.Setup(m => m.GetAllPlayers(testChannel)).Returns(new List<KPlayer>());
            else
                returnDb.Setup(m => m.GetAllPlayers(testChannel)).Returns(playerList);

            return returnDb;
        }
    }
}