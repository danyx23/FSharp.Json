﻿namespace FSharp.Json

module Union =
    open NUnit.Framework

    type TheRecord = {
        Value: string
    }

    type TheUnion =
    | OneFieldCase of string
    | ManyFieldsCase of string*int
    | RecordCase of TheRecord

    [<Test>]
    let ``Union one field case serialization`` () =
        let value = OneFieldCase "The string"
        let actual = Json.serializeU value
        let expected = """{"OneFieldCase":"The string"}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Union one field case deserialization`` () =
        let expected = OneFieldCase "The string"
        let json = Json.serialize(expected)
        let actual = Json.deserialize<TheUnion> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Union many fields case serialization`` () =
        let expected = ManyFieldsCase ("The string", 123)
        let json = Json.serialize(expected)
        let actual = Json.deserialize<TheUnion> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Union record field case serialization`` () =
        let expected = RecordCase {TheRecord.Value = "The string"}
        let json = Json.serialize(expected)
        let actual = Json.deserialize<TheUnion> json
        Assert.AreEqual(expected, actual)

    type TheCasesUnion =
    | [<JsonUnionCase(Case="case1")>] StringCase of string
    | [<JsonUnionCase(Case="case2")>] IntCase of int

    [<Test>]
    let ``Union custom case name serialization`` () =
        let value = StringCase "The string"
        let actual = Json.serializeU value
        let expected = """{"case1":"The string"}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Union custom case name deserialization`` () =
        let expected = StringCase "The string"
        let json = """{"case1":"The string"}"""
        let actual = Json.deserialize<TheCasesUnion> json
        Assert.AreEqual(expected, actual)

    [<JsonUnion(Mode = UnionMode.CaseKeyAsFieldValue, CaseKeyField="casekey", CaseValueField="casevalue")>]
    type TheAnnotatedUnion =
    | StringCase of string
    | IntCase of int

    [<Test>]
    let ``Union key-value serialization`` () =
        let value = TheAnnotatedUnion.StringCase "The string"
        let actual = Json.serializeU value
        let expected = """{"casekey":"StringCase","casevalue":"The string"}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Union key-value deserialization`` () =
        let expected = TheAnnotatedUnion.StringCase "The string"
        let json = """{"casekey":"StringCase","casevalue":"The string"}"""
        let actual = Json.deserialize<TheAnnotatedUnion> json
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Union cases serialization with snake case naming`` () =
        let value = OneFieldCase "The string"
        let config = JsonConfig.create(jsonFieldNaming = Json.snakeCase)
        let actual = Json.serializeExU config value
        let expected = """{"one_field_case":"The string"}"""
        Assert.AreEqual(expected, actual)
        
    [<Test>]
    let ``Union cases deserialization with snake case naming`` () =
        let json = """{"one_field_case":"The string"}"""
        let expected = OneFieldCase "The string"
        let config = JsonConfig.create(jsonFieldNaming = Json.snakeCase)
        let actual = Json.deserializeEx<TheUnion> config json
        Assert.AreEqual(expected, actual)

    type SingleCaseUnion = SingleCase of string

    type SingleCaseRecord = {
        value: SingleCaseUnion
    }

    [<Test>]
    let ``Union single case serialization`` () =
        let value = { SingleCaseRecord.value = SingleCase "The string" }
        let actual = Json.serializeU value
        let expected = """{"value":"The string"}"""
        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Union single case deserialization`` () =
        let expected = { SingleCaseRecord.value = SingleCase "The string" }
        let json = Json.serialize(expected)
        let actual = Json.deserialize<SingleCaseRecord> json
        Assert.AreEqual(expected, actual)