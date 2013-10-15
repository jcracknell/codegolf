import org.scalatest.{FunSpec, Matchers}

class SequentialUuidGeneratorSpecs extends FunSpec with Matchers {
  describe("SequentialUuidGenerator") {
    it("should generate unique Uuid values") {
      val generator = new SequentialUuidGenerator
      var seen = scala.collection.mutable.HashSet[Uuid]()
      for(i <- 1 to 10000)
        (seen.add(generator.generate())) should be (true)
    }
    it("should generate sequential Uuid values") {
      val generator = new SequentialUuidGenerator

      var last = Uuid.nil
      for(i <- 1 to 1000) {
        val generated = generator.generate()
        generated should be > last
        last = generated
      }
    }
    it("should generate version 1 Uuid values") {
      (new SequentialUuidGenerator().generate().version) should be (1)
    }
    it("should set the multicast bit") {
      val MulticastBit = 0x010000000000l

      for(i <- 1 to 1000)
        (new SequentialUuidGenerator().generate().node & MulticastBit) should be (MulticastBit)
    }
    it("should be fast") {
      val iterations = 1000000

      // Construct and prime the generator
      val generator = new SequentialUuidGenerator
      generator.generate()

      val start = System.currentTimeMillis()

      for(i <- 1 to iterations)
        generator.generate()

      val elapsed = System.currentTimeMillis() - start
      val rate = 1000 * iterations.toDouble / elapsed

      (rate) should be >= 10000d
    }
  }
}
