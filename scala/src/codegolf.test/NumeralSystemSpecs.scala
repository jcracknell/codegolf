import org.scalatest.FunSpec
import org.scalatest.matchers.ShouldMatchers

class NumeralSystemSpecs extends FunSpec with ShouldMatchers {
  describe("NumeralSystem") {
    describe("Alpha") {
      describe("decode method") {
        it("should decode a") {
          NumeralSystem.Alpha.decode("a") should be (Some(1))
        }
        it("should decode b") {
          NumeralSystem.Alpha.decode("b") should be (Some(2))
        }
        it("should decode z") {
          NumeralSystem.Alpha.decode("z") should be (Some(26))
        }
        it("should decode aa") {
          NumeralSystem.Alpha.decode("aa") should be (Some(27))
        }
        it("should decode AA") {
          NumeralSystem.Alpha.decode("AA") should be (Some(27))
        }
      }
      describe("encode method") {
        it("should return None for 0") {
          NumeralSystem.Alpha.encode(0) should be (None)
        }
        it("should return None for -1") {
          NumeralSystem.Alpha.encode(-1) should be (None)
        }
        it("should encode 1") {
          NumeralSystem.Alpha.encode(1) should be (Some("a"))
        }
        it("should encode 2") {
          NumeralSystem.Alpha.encode(2) should be (Some("b"))
        }
        it("should encode 26") {
          NumeralSystem.Alpha.encode(26) should be (Some("z"))
        }
        it("should encode 27") {
          NumeralSystem.Alpha.encode(27) should be (Some("aa"))
        }
        it("should encode 53") {
          NumeralSystem.Alpha.encode(53) should be (Some("ba"))
        }
        it("should encode 54") {
          NumeralSystem.Alpha.encode(54) should be (Some("bb"))
        }
        it("should encode 153") {
          NumeralSystem.Alpha.encode(153) should be (Some("ew"))
        }
        it("should encode 267") {
          NumeralSystem.Alpha.encode(267) should be (Some("jg"))
        }
        it("should encode 329") {
          NumeralSystem.Alpha.encode(329) should be (Some("lq"))
        }
        it("should encode 17603") {
          NumeralSystem.Alpha.encode(17603) should be (Some("zaa"))
        }
        it("should encode Int.MaxValue") {
          NumeralSystem.Alpha.encode(Int.MaxValue) should be (Some("fxshrxw"))
        }
      }
    }
  }
}
