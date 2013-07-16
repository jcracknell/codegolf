import org.scalatest.FunSpec
import org.scalatest.matchers.ShouldMatchers

class LevenshteinDistanceSpec extends FunSpec with ShouldMatchers {
  describe("LevenshteinDistance result") {
    it("should be 0 for identical strings") {
      LevenshteinDistance.compute("james","james") should be (0)
    }
    it("should be 3 for kitten/sitting") {
      LevenshteinDistance.compute("kitten","sitting") should be (3)
    }
  }
}
