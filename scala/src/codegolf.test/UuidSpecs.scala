import org.scalatest.FunSpec
import org.scalatest.matchers.ShouldMatchers

class UuidSpecs extends FunSpec with ShouldMatchers {
  describe("fields") {
    // The following Uuid instance has a (trivially) distinctive bit pattern which makes it useful
    // for testing purposes
    val bitPattern = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)

    it("timeLow should extract the correct value") {
      bitPattern.timeLow should be (0x01234567)
    }
    it("timeMid should extract the correct value") {
      bitPattern.timeMid should be (0x89ab.toShort)
    }
    it("timeHiAndVersion should extract the correct value") {
      bitPattern.timeHiAndVersion should be (0xcdef.toShort)
    }
    it("clkSeqHiRes should extract the correct value") {
      bitPattern.clkSeqHiRes should be (0xfe.toByte)
    }
    it("clkSeqLow should extract the correct value") {
      bitPattern.clkSeqLow should be (0xdc.toByte)
    }
    it("node should extract the correct value") {
      bitPattern.node should be (0xba9876543210l)
    }
    it("timestamp should extract the correct value") {
      bitPattern.timestamp should be (0x0def89ab01234567l)
    }
  }
  describe("toByteArray method") {
    it("should output the correct bytes") {
      val u = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      val bs = Array(0x01,0x23,0x45,0x67,0x89,0xab,0xcd,0xef,0xfe,0xdc,0xba,0x98,0x76,0x54,0x32,0x10).map(_.toByte)
      u.toByteArray should be (bs)
    }
  }
  describe("toString") {
    it("should format correctly") {
      new Uuid(0x0123456789abcdefl,0x0123456789abcdefl).toString should be ("01234567-89ab-cdef-0123-456789abcdef")
    }
    it("should format bitPattern correctly") {
      new Uuid(0x0123456789abcdefl,0xfedcba9876543210l).toString should be ("01234567-89ab-cdef-fedc-ba9876543210")
    }
  }
  describe("equality") {
    it("should be true for instances with the same values") {
      val a = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      val b = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      (a == b) should be (true)
    }
    it("should be false for instances with differing values") {
      val a = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      val b = new Uuid(0x0123456789abcdefl,0x0123456789abcdefl)
      (a == b) should be (false)
    }
  }
  describe("ordering") {
    it("should be equivalent for instances with the same value") {
      val a = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      val b = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      (a compare b) should be (0)
    }
    it("should not be equivalent for instances with differing values") {
      val a = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      val b = new Uuid(0x0123456789abcdefl,0x0123456789abcdefl)
      (a compare b) should not be (0)
    }
    describe("of values composed of positive signed longs") {
      it("should consider timeHi more significant than timeMid") {
        val a = new Uuid(0x0808080808070809l,0x0808080808080808l)
        val b = new Uuid(0x0808080808080808l,0x0808080808080808l)
        a should be > b
        b should be < a
      }
      it("should consider timeMid more significant than timeLo") {
        val a = new Uuid(0x0808080708090808l,0x0808080808080808l)
        val b = new Uuid(0x0808080808080808l,0x0808080808080808l)
        a should be > b
        b should be < a
      }
      it("should consider timeLo more significant than clkSeqHiRes") {
        val a = new Uuid(0x0808080908080808l,0x0708080808080808l)
        val b = new Uuid(0x0808080808080808l,0x0808080808080808l)
        a should be > b
        b should be < a
      }
      it("should consider clkSeqHiRes more significant than clkSeqLow") {
        val a = new Uuid(0x0808080808080808l,0x0907080808080808l)
        val b = new Uuid(0x0808080808080808l,0x0808080808080808l)
        a should be > b
        b should be < a
      }
      it("should consider clkSeqLow more significant than node") {
        val a = new Uuid(0x0808080808080808l,0x0809080808080807l)
        val b = new Uuid(0x0808080808080808l,0x0808080808080808l)
        a should be > b
        b should be < a
      }
    }
    describe("of values composed of negative signed longs") {
      it("should consider timeHi more significant than timeMid") {
        val a = new Uuid(0xF8F8F8F8F8F7F8F9l,0xF8F8F8F8F8F8F8F8l)
        val b = new Uuid(0xF8F8F8F8F8F8F8F8l,0xF8F8F8F8F8F8F8F8l)
        a should be > b
        b should be < a
      }
      it("should consider timeMid more significant than timeLo") {
        val a = new Uuid(0xF8F8F8F7F8F9F8F8l,0xF8F8F8F8F8F8F8F8l)
        val b = new Uuid(0xF8F8F8F8F8F8F8F8l,0xF8F8F8F8F8F8F8F8l)
        a should be > b
        b should be < a
      }
      it("should consider timeLo more significant than clkSeqHiRes") {
        val a = new Uuid(0xF8F8F8F9F8F8F8F8l,0xF7F8F8F8F8F8F8F8l)
        val b = new Uuid(0xF8F8F8F8F8F8F8F8l,0xF8F8F8F8F8F8F8F8l)
        a should be > b
        b should be < a
      }
      it("should consider clkSeqHiRes more significant than clkSeqLow") {
        val a = new Uuid(0xF8F8F8F8F8F8F8F8l,0xF9F7F8F8F8F8F8F8l)
        val b = new Uuid(0xF8F8F8F8F8F8F8F8l,0xF8F8F8F8F8F8F8F8l)
        a should be > b
        b should be < a
      }
      it("should consider clkSeqLow more significant than node") {
        val a = new Uuid(0xF8F8F8F8F8F8F8F8l,0xF8F9F8F8F8F8F8F7l)
        val b = new Uuid(0xF8F8F8F8F8F8F8F8l,0xF8F8F8F8F8F8F8F8l)
        a should be > b
        b should be < a
      }
    }
  }
  describe("Array[Byte] constructor") {
    it("should contruct the correct Uuid") {
      val expected = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      val actual = Uuid(Array(0x01,0x23,0x45,0x67,0x89,0xab,0xcd,0xef,0xfe,0xdc,0xba,0x98,0x76,0x54,0x32,0x10).map(_.toByte))
      actual should be (expected)
    }
    it("should throw an exception if there are too few bytes") {
      intercept[AssertionError] {
        Uuid(Array(0x01,0x23,0x45,0x67,0x89,0xab,0xcd,0xef,0xfe,0xdc,0xba,0x98,0x76,0x54,0x32).map(_.toByte))
      }
    }
    it("should throw an exception if there are too many bytes") {
      intercept[AssertionError] {
        Uuid(Array(0x01,0x23,0x45,0x67,0x89,0xab,0xcd,0xef,0xfe,0xdc,0xba,0x98,0x76,0x54,0x32,0x10,0x01).map(_.toByte))
      }
    }
  }
  describe("parsing constructor") {
    it("should parse toString results") {
      val prototype = new Uuid(0x0123456789abcdefl,0xfedcba9876543210l)
      val parsed = Uuid(prototype.toString)
      parsed.isDefined should be (true)
      parsed.get should be (prototype)
    }
    it("should parse with hyphens") {
      val parsed = Uuid("01234567-89ab-cdef-fedc-ba9876543210")
      parsed.isDefined should be (true)
      parsed.get should be (new Uuid(0x0123456789abcdefl,0xfedcba9876543210l))
    }
    it("should parse without hyphens") {
      val parsed = Uuid("0123456789abcdeffedcba9876543210")
      parsed.isDefined should be (true)
      parsed.get should be (new Uuid(0x0123456789abcdefl,0xfedcba9876543210l))
    }
    it("should parse with curly braces") {
      val parsed = Uuid("{01234567-89ab-cdef-fedc-ba9876543210}")
      parsed.isDefined should be (true)
      parsed.get should be (new Uuid(0x0123456789abcdefl,0xfedcba9876543210l))
    }
  }
}
