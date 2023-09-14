namespace PostingService.UnitTests;
using StatusAPI;
using MongoDB.Driver;
using Moq;
using StatusDefinition;

public class StatusAPITests
{
    //Aim: Test the individual functions/units of the system using mock/fake interfaces
    Mock<IMongoClient> mockMongoClient = new Mock<IMongoClient>();
    Mock<IMongoDatabase> mockMongoDatabase = new Mock<IMongoDatabase>();
    Mock<IMongoCollection<Status>> mockMongoCollection = new Mock<IMongoCollection<Status>>();

    public StatusAPITests() {

        // Status product = new Status("Unit Test Title","Unit Test Content",new StatusLocation(){
        //         City = "Test City",
        //         Region = "Test Region",
        //         Country = ""
        //     }){Id = ""};
        // Status status = new Status{
        //     Id = "",
        //     Title = statusRequest.Title,
        //     Content = statusRequest.Message,
        //     Location = location
        // };
    }

    
    [Fact]
    public void Test1()
    {
        //Arrange
        // Arrange
        var mockMongoCollection = new Mock<IMongoCollection<Status>>();
        var mockMongoDatabase = new Mock<IMongoDatabase>();
        mockMongoDatabase.Setup(db => db.GetCollection<Status>("status", null))
                         .Returns(mockMongoCollection.Object);
        //Act

        var ret = StatusAPI.GetStatuses(mockMongoCollection.Object, 1,1);
        //ASSERT
        Assert.NotNull(ret);
    }
}