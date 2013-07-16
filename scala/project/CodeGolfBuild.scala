import sbt._
import Keys._

object Dependencies {
  def scalaReflect(sv:String) = "org.scala-lang"     % "scala-reflect"     % sv
  def scalatest               = "org.scalatest"      % "scalatest_2.10"    % "1.9.1"
}

object CodeGolfBuild extends Build {
  import Dependencies._

  lazy val codegolf = Project(
    id       = "codegolf",
    base     = file("."),
    settings = Defaults.defaultSettings ++ Seq(
               version             := "0.1",
               scalaVersion        := "2.10.2",
               libraryDependencies <++= (scalaVersion){ sv => Seq(
                                     scalaReflect(sv),
                                     scalatest % "test"
                                   )}
             )
  )
}
