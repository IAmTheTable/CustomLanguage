using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
public class Config{
  public static char EndOfLine = ';';
  public static char define = '=';
  public static char CharCode = '\'';
  public static char StringCode = '"';
}
class MainClass {

  public static int currentLine = 0;
  public enum TokenType{
    Any,
    String,
    Int,
    Bool,
    Char
  }
  public enum ExpressionType{
    Print,
    Add,
    Subtract
  }

  public static bool isString(object input){
    if(input.ToString().Contains("\"")){
      return true;
    }
    else
      return false;
  }
    public static bool isBool(object input){
      if(input.ToString() == "true" || input.ToString() == "false"){
        return true;
      }
      else{
        return false;
      }
  }
    public static bool isInt(object input){
      try{
        input = int.Parse(input.ToString());
        return true;
      }
      catch(Exception){
        return false;
      }
      
    }
    public static bool isChar(object input){
      input = char.Parse(input.ToString());
    if(input.GetType() == typeof(char)){
      return true;
    }
    else
      return false;
  }

  public static Dictionary<Tuple<TokenType,TokenType,string>,string> data = new Dictionary<Tuple<TokenType,TokenType,string>,string>();

  public static Dictionary<Tuple<ExpressionType,string>,string[]> expressionData = new Dictionary<Tuple<ExpressionType,string>,string[]>();
  public static Tuple<bool,string> tryGetValueFromData(object input){
    
    //Console.WriteLine("got input " + input);
    try{
      var TupleInfo = Tuple.Create(TokenType.Any, TokenType.String, input.ToString());
      return Tuple.Create(true,(data[TupleInfo]));
      //Console.WriteLine(data[TupleInfo]);
    }catch(Exception){
      try{
        var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Int, input.ToString());
        return Tuple.Create(true,(data[TupleInfo]));
        //Console.WriteLine("Info " + data[TupleInfo]);
      }catch(Exception){
        try{
            var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Bool, input.ToString());
            return Tuple.Create(true,(data[TupleInfo]));            //Console.WriteLine(data[TupleInfo]);
        }
        catch(Exception){
          try{
            var TupleInfo = Tuple.Create(TokenType.Any, TokenType.Char, input.ToString());
            return Tuple.Create(true,(data[TupleInfo]));
            //Console.WriteLine(data[TupleInfo]);
          }
          catch(Exception){
            return Tuple.Create(false,"");
            //throw new Exception("Value not in data table.");
          }
        }
      }
    }
  }
public static bool Recompile(string input){
  try{
    foreach(var Semi in input.Split(Config.EndOfLine)){
    //Define the variables into the data table
      switch(Semi.Split(" ")[0]){
        case "string":
          var key = Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
          if(key.StartsWith(" ")){
            key = key.Substring(1);
          }
          //Console.WriteLine("Key pt " + key + " " + isString(key));
          if(isString(key)){
            var tupleData = Tuple.Create(TokenType.Any,TokenType.String,Semi.Split(" ")[1].Split(Config.define)[0]);
            //Console.WriteLine("key pt2 " + tupleData);
            data.Add(tupleData,key);
            break;
          }
          else{
            //Console.WriteLine("xd" + Semi.Split("string")[1].Split(" ")[1].Split(" ")[0]);
            var data2 = tryGetValueFromData(key);
            //Console.WriteLine("PreKek2" + key);
            //Console.WriteLine("KEK2" + data2);
            var TupleData = Tuple.Create(TokenType.Any,TokenType.String,Semi.Split("string")[1].Split(" ")[1].Split(" ")[0]);
            data.Add(TupleData, data2.Item2);
            break;
          }
        case "int":
          var intkey = Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
          if(intkey.StartsWith(" ")){
            intkey = intkey.Substring(1);
          }
          if(isInt(Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]))
          {
            //Console.WriteLine("k"+key);
            var TupleData = Tuple.Create(TokenType.Any, TokenType.Int, Semi.Split(" ")[1].Split(Config.define)[0]);
            data.Add(TupleData,intkey);
            break;
          }
          else{
            //Console.WriteLine("heyo");
            var TupleData = Tuple.Create(TokenType.Any, TokenType.Int, Semi.Split(" ")[1].Split(Config.define)[0]);
            //Console.WriteLine("trying " + key);
            try{
              if(tryGetValueFromData(intkey).Item1 == true){
                data.Add(TupleData, tryGetValueFromData(intkey).Item2);
              }
              else{
                throw new Exception("Variable not found or initialized.");
              }
            }
            catch(Exception){}
            break;
          }
        case "bool":
          var value = Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
          if(value.StartsWith(" ")){
            value = value.Substring(1);
          }
          var varNameB = Semi.Split("bool ")[1].Split(" ")[0];
          if(isBool(value) == true){
            var TupleData = Tuple.Create(TokenType.Any, TokenType.Bool, varNameB);
            data.Add(TupleData,Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]);
            break;
          }
          else if(tryGetValueFromData(value).Item1 == true){
            var TupleData = Tuple.Create(TokenType.Any, TokenType.Bool, varNameB);
            data.Add(TupleData,tryGetValueFromData(value).Item2);
            break;
          }
          else{
            throw new Exception("Invalid type of bool at " + Semi.Split(" ")[1].Split(Config.define)[0] + " line: " + currentLine + " reason: " + value + " is either not defined or isnt of type 'bool'.");
          }
        case "char":
          /*var key = Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0];
          if(key.StartsWith(" ")){
            key = key.Substring(1);
          }*/
          if(Semi.Split("=")[1].Contains("'")){
            if(Semi.Split("'")[1].Split("'")[0].Length != 1){
              throw new Exception("LOL CHAR");
            }
            else{
              var TupleData = Tuple.Create(TokenType.Any, TokenType.Char, Semi.Split(" ")[1].Split(Config.define)[0]);
              data.Add(TupleData,Semi.Split(Config.define)[1].Split(Config.EndOfLine)[0]);
              break;
            }
          }
          else{
            throw new Exception("LOL CHAR");
          }
      }

      //Run the expressions
      switch(Semi.Split(Config.EndOfLine)[0].Split("(")[0]){
        case "Print":
          var data = Semi.Split(Config.EndOfLine)[0].Split("(")[1].Split(")")[0];
          data.Remove(" ", "");
          var TupleData = Tuple.Create(ExpressionType.Print,Semi.Split("Print")[1].Split(Config.EndOfLine)[0]);
          var extraArgs = new string[1];
          expressionData.Add(TupleData,extraArgs);
          break;
        default:
          break;
      }
    }
    return true;
  }
  catch(Exception exx){
    Console.WriteLine(exx.StackTrace + "\n" + exx.Message);
    return true;
  }
}
public static bool ExecuteExpression(){
  foreach(var exp in expressionData.Keys){
    //Console.WriteLine("Item val " + exp.Item2);
    switch(exp.Item1){
      case ExpressionType.Print:
        if(exp.Item2.Contains("\"")){
          Console.WriteLine(exp.Item2.Split("\"")[1].Split("\"")[0]);
        }
        else if(!exp.Item2.Contains("\"") || !exp.Item2.Contains("'")){
          if(tryGetValueFromData(exp.Item2.Split("(")[1].Split(")")[0].ToString()).Item1 == true){
            Console.WriteLine(tryGetValueFromData(exp.Item2.Split("(")[1].Split(")")[0].ToString()).Item2);
          }
        }
        break;
      default: 
        break;
    }
  }
  return true;
}

  public static void Main (string[] args) {
    string[] input = File.ReadAllLines("main.TFG");
    foreach(var s in input){
      //Console.WriteLine(s);
      currentLine++;
      Recompile(s);
    }
    foreach(var item in data.Keys){
      //Console.WriteLine(item + " - " + data[item]);
    }    
    ExecuteExpression();
  }
}