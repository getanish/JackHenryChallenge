namespace JackHenryChallenge.Tests.Models
{
    public class HashTagTests
    {
        [Fact]
        public void HashTag_MatchesEqualTags()
        {
            var a = new HashTag() { Tag="gn"};
            var b = new HashTag() { Tag = "gn" };
            Assert.Equal(a, b);

        }

        [Fact]
        public void HashTag_MatchesCaseInsensitiveEqualTags()
        {
            var a = new HashTag() { Tag = "GN" };
            var b = new HashTag() { Tag = "gn" };
            Assert.Equal(a, b);

        }
        [Fact]
        public void HasTag_DoesNotMatchUnEqualTags()
        {
            var a = new HashTag() { Tag = "GN " };
            var b = new HashTag() { Tag = "gn" };
            Assert.NotEqual(a, b);

        }

        [Fact]
        public void HashTags_MatchesEqualObject()
        {
            var a = new HashTag() { Tag = "GN" };
            object b = new HashTag() { Tag = "gn" };
            Assert.Equal(a, b);

        }
        [Fact]
        public void HashTags_DoesNotMatachDifferentObjects()
        {
            var a = new HashTag() { Tag = "GN" };
            var b = "gn";
            Assert.False(  a.Equals(b));

        }
        [Fact]
        public void HashTag_DoesNotMatchNull()
        {
            var a = new HashTag() { Tag = "GN" };
            Assert.False(a.Equals(null));

        }

        [Fact]
        public void HashTags_GetsTheCorrectString()
        {
            var a = new HashTag() { Tag = "GN" };
            Assert.Equal("GN", a.ToString());

        }
    }
}
