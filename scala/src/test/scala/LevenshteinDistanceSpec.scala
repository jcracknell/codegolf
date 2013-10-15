import org.scalatest.{FunSpec, Matchers}

class LevenshteinDistanceSpec extends FunSpec with Matchers {
  describe("LevenshteinDistance result") {
    it("should be 0 for identical strings") {
      LevenshteinDistance.compute("james","james") should be (0)
    }
    it("should be 3 for kitten/sitting") {
      LevenshteinDistance.compute("kitten","sitting") should be (3)
    }
  }
}
