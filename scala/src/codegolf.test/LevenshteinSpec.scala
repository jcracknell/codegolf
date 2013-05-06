import java.util.Date
import org.scalatest.FunSpec
import org.scalatest.matchers.ShouldMatchers

class LevenshteinSpec extends FunSpec with ShouldMatchers {
  describe("Levenshtein distance implementation") {
    describe("result") {
      it("should be 0 for identical strings") {
        Levenshtein.levenshtein("james","james") should be (0)
      }
      it("should be 3 for kitten/sitting") {
        Levenshtein.levenshtein("kitten","sitting") should be (3)
      }
    }
  }
}
