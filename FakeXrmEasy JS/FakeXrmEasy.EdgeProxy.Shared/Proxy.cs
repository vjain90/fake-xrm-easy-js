using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;

namespace FakeXrmEasy.EdgeProxy
{
    public class Proxy
    {
        static XrmFakedContext ctx = null;

        protected QueryExpression ConvertQueryFromDynamic(dynamic query)
        {
            var queryProperties = query as Dictionary<string, object>;

            var qe = new QueryExpression();
            qe.EntityName = query.EntityName as string;

            if (Extensions.DynamicContainsProperty(query, "ColumnSet") &&
                query.ColumnSet is object[])
            {
                var cols = query.ColumnSet as object[];
                qe.ColumnSet = new ColumnSet(cols.Select(c => c.ToString()).ToArray());
            }
            else
            {
                qe.ColumnSet = new ColumnSet(true);
            }

            //Criteria
            if (Extensions.DynamicContainsProperty(query, "Criteria") &&
                query.Criteria != null)
            {
                var criteria = query.Criteria as IDictionary<string, object>;
                qe.Criteria = ConvertFilterExpressionFromDynamic(criteria);
            }

            //Top Count
            if (Extensions.DynamicContainsProperty(query, "TopCount") &&
                query.TopCount != null)
            {
                qe.TopCount = (query.TopCount as int?).Value;
            }
            return qe;
        }



        protected bool IsBooleanOperator(string type)
        {
            return type == "and" || type == "or" || type == "not";
        }

        protected bool IsFunctionCall(string type)
        {
            return type == "functioncall";
        }

        protected bool IsRelationalOperator(string type)
        {
            return !IsBooleanOperator(type)
                    && !IsLiteral(type)
                    && !IsFunctionCall(type);
        }

        protected bool IsLiteral(string type)
        {
            return type == "literal";
        }

        protected ConditionExpression ConvertRelationalExpressionFromDynamic(dynamic condition)
        {
            var type = condition.type as string;

            var isProperty = (condition.left.type as string).Equals("property");
            if (!isProperty)
            {
                throw new Exception("Condition expression must have a property in the left hand side of the expression");
            }

            /*
            var isLiteral = (condition.right.type as string).Equals("literal");
            if (!isLiteral)
            {
                throw new Exception("Condition expression must have a literal in the right hand side of the expression");
            }
            */
            var newCondition = new ConditionExpression();
            object conditionVal = (condition.right.type as string).Equals("literal") ? condition.right.value : condition.right.name;
            Guid conditionValue;

            if (Guid.TryParse(conditionVal as string, out conditionValue))
            {
                newCondition = new ConditionExpression(condition.left.name as string, ConditionOperator.Equal, conditionValue as object);
            }
            else
            {
                newCondition = new ConditionExpression(condition.left.name as string, ConditionOperator.Equal, conditionVal as object);
            }

            switch (type)
            {
                case "eq":
                    //Equals
                    newCondition.Operator = ConditionOperator.Equal;
                    return newCondition;

                case "ne":
                    //Equals
                    newCondition.Operator = ConditionOperator.NotEqual;
                    return newCondition;

                case "gt":
                    //Equals
                    newCondition.Operator = ConditionOperator.GreaterThan;
                    return newCondition;

                case "ge":
                    //Equals
                    newCondition.Operator = ConditionOperator.GreaterEqual;
                    return newCondition;

                case "lt":
                    //Equals
                    newCondition.Operator = ConditionOperator.LessThan;
                    return newCondition;

                case "le":
                    //Equals
                    newCondition.Operator = ConditionOperator.LessEqual;
                    return newCondition;

                default:

                    throw new Exception(string.Format("{0} operator not yet supported", type));
            }
        }

        protected bool IsPropertyType(string type)
        {
            return type.Equals("property");
        }

        protected bool IsLiteralType(string type)
        {
            return type.Equals("literal");
        }

        protected ConditionExpression ConvertFunctionCallFromDynamic(dynamic filter)
        {
            var type = filter.func as string;


            var isLeftAProperty = IsPropertyType((filter.args[0].type as string));
            var isLeftALiteral = IsLiteralType((filter.args[0].type as string));

            var isRightAProperty = IsPropertyType((filter.args[1].type as string));
            var isRightALiteral = IsLiteralType((filter.args[1].type as string));

            if (type.Equals("substringof"))
            {
                if (!isLeftALiteral || !isRightAProperty)
                {
                    throw new Exception("Condition expression 'substringof' must have a literal in the left hand side of the expression and a property in the right hand side.");
                }
            }
            else
            {
                if (!isLeftAProperty || !isRightALiteral)
                {
                    throw new Exception("Condition expression must have a property in the left hand side of the expression and a literal in the right hand side.");
                }
            }

            string property;
            object literal;

            if (isLeftAProperty)
                property = filter.args[0].name as string;
            else
                property = filter.args[1].name as string;

            if (isLeftALiteral)
                literal = filter.args[0].value as object;
            else
                literal = filter.args[1].value as object;


            var newCondition = new ConditionExpression(property, ConditionOperator.Equal, literal);
            switch (type)
            {
                case "startswith":
                    //Equals
                    newCondition.Operator = ConditionOperator.BeginsWith;
                    return newCondition;

                case "endswith":
                    //Equals
                    newCondition.Operator = ConditionOperator.EndsWith;
                    return newCondition;

                case "substringof":
                    //Equals
                    literal = string.Format("%{0}%", literal);
                    newCondition.Values[0] = literal;
                    newCondition.Operator = ConditionOperator.Like;
                    return newCondition;


                default:

                    throw new Exception(string.Format("{0} function not yet supported", type));
            }
        }

        protected ConditionExpression NegateCondition(ConditionExpression condition)
        {
            var newCondition = new ConditionExpression(condition.AttributeName, condition.Operator, condition.Values);

            switch (condition.Operator)
            {
                case ConditionOperator.Equal:
                    newCondition.Operator = ConditionOperator.NotEqual;
                    break;
                case ConditionOperator.NotEqual:
                    newCondition.Operator = ConditionOperator.Equal;
                    break;

                default:
                    throw new Exception(string.Format("Negate condition for operator {0} not yet supported.", condition.Operator.ToString()));
            }

            return newCondition;
        }

        protected FilterExpression NegateExpression(dynamic filter)
        {
            var type = filter.type as string;

            if (type == "not")
            {
                //Recursive call
                return NegateExpression(filter.left);
            }

            if (IsRelationalOperator(type))
            {
                //Negate operator
                return NegateCondition(ConvertRelationalExpressionFromDynamic(filter.left));
            }
            else // (IsBooleanOperator(type))
            {
                //Swap logical operator and negate operands (this is pure Bool algebra)
                var left = NegateExpression(ConvertFilterExpressionFromDynamic(filter.left));
                var right = NegateExpression(ConvertFilterExpressionFromDynamic(filter.right));

                var filterExp = new FilterExpression();
                switch (type)
                {
                    case "or":
                        filterExp.FilterOperator = LogicalOperator.And;
                        break;
                    case "and":
                        filterExp.FilterOperator = LogicalOperator.Or;
                        break;
                }
                return filterExp;
            }
        }

        protected FilterExpression ConvertFilterExpressionFromDynamic(dynamic filter)
        {
            var type = filter.type as string;

            if (IsRelationalOperator(type))
            {
                //Filter with a single condition expression
                var condition = ConvertRelationalExpressionFromDynamic(filter);
                var f = new FilterExpression(LogicalOperator.And);
                f.Conditions.Add(condition);
                return f;
            }

            else if (IsFunctionCall(type))
            {
                //Filter with a single condition expression
                var condition = ConvertFunctionCallFromDynamic(filter);
                var f = new FilterExpression(LogicalOperator.And);
                f.Conditions.Add(condition);
                return f;
            }

            else if (IsBooleanOperator(type))
            {
                //Process child filters recursively
                FilterExpression left = null, right = null;

                var filterExp = new FilterExpression();
                switch (type)
                {
                    case "or":
                        filterExp.FilterOperator = LogicalOperator.Or;
                        left = ConvertFilterExpressionFromDynamic(filter.left);
                        right = ConvertFilterExpressionFromDynamic(filter.right);
                        break;
                    case "and":
                        filterExp.FilterOperator = LogicalOperator.And;
                        left = ConvertFilterExpressionFromDynamic(filter.left);
                        right = ConvertFilterExpressionFromDynamic(filter.right);
                        break;
                    case "not":
                        left = NegateExpression(filter.left);
                        break;
                }

                filterExp.Filters.Add(left);
                if (right != null)
                    filterExp.Filters.Add(right);  //Optional for "not"

                return filterExp;
            }

            else
            {
                throw new Exception("Invalid criteria: it must be either a condition or a filterexpression.");
            }
        }

        protected Entity ConvertEntityFromDynamic(dynamic entityWrapper)
        {
            var e = new Entity(entityWrapper.EntityName as string);



            //Convert all attributes
            var attributes = entityWrapper.Entity as IDictionary<string, object>;


            //Get Id property
            if (attributes.ContainsKey("id"))
                e.Id = new Guid(attributes["id"] as string);


            foreach (var sKey in attributes.Keys)
            {
                if (sKey != "id")
                    e[sKey] = ConvertAttributeValueFromDynamic(attributes[sKey]);
            }

            return e;
        }

        protected object ConvertAttributeValueFromDynamic(object value)
        {
            //Basic types
            if (value is string)
            {
                return ConvertAttributeValueFromString(value as string);
            }
            else if (value is int
                || value is decimal
                || value is double
                || value is float
                || value is bool)
            {
                return value;
            }

            //Non-basic type
            var expando = value as System.Dynamic.ExpandoObject;
            if (expando.IsEntityReference())
            {
                //Entity Reference
                return new EntityReference()
                {
                    Id = new Guid(expando.GetKeyValue("id").ToString()),
                    LogicalName = expando.GetKeyValue("logicalName") as string
                };
            }

            return null;
        }

        protected object ConvertAttributeValueFromString(string value)
        {
            //Try to parse as Guid
            Guid g = Guid.Empty;
            if (Guid.TryParse(value, out g))
            {
                return g;
            }

            return value;
        }

        public async Task<object> PrepareContext(dynamic input)
        {
            try
            {
                return ctx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<object> InitializeContext(dynamic input)
        {
            try
            {
                ctx = new XrmFakedContext();

                var entities = input.Entities as object[];

                var listOfEntities = new List<Entity>();
                foreach (var entity in entities)
                {
                    listOfEntities.Add(ConvertEntityFromDynamic(entity));
                }

                ctx.Initialize(listOfEntities);

                return ctx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<object> TranslateODataQueryToQueryExpression(dynamic input)
        {
            try
            {
                var service = ctx.GetOrganizationService();

                var result = new EntityCollection();

                if (!string.IsNullOrEmpty(input.FetchXml as string))
                    result = service.RetrieveMultiple(new FetchExpression(input.FetchXml)) as EntityCollection;
                else
                {
                    var query = ConvertQueryFromDynamic(input.QueryExpression);
                    result = service.RetrieveMultiple(query) as EntityCollection;
                }

                Dictionary<string, object> lookupAttributeCollection = new Dictionary<string, object>();
                List<string> attrToRemove = new List<string>();
                List<KeyValuePair<string, object>> attrToAdd = new List<KeyValuePair<string, object>>();

                foreach (Entity en in result.Entities)
                {
                    foreach (KeyValuePair<string, object> attr in en.Attributes.Where(w => w.Value is EntityReference))
                    {
                        attrToAdd.Add(new KeyValuePair<string, object>("_" + attr.Key + "_value", ((EntityReference)attr.Value).Id));
                        attrToRemove.Add(attr.Key);
                    }

                    foreach (string attr in attrToRemove)
                        en.Attributes.Remove(attr);

                    foreach (KeyValuePair<string, object> attr in attrToAdd)
                        en.Attributes.Add(attr);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> DefineNNRelationships(dynamic input)
        {
            try
            {
                var relationshipName = ConvertAttributeValueFromDynamic(input.RelationshipName);
                var intersectEntity = ConvertAttributeValueFromDynamic(input.IntersectEntity);
                var entityName = ConvertAttributeValueFromDynamic(input.EntityName);
                var entityAttribute = ConvertAttributeValueFromDynamic(input.EntityAttribute);
                var relatedEntityName = ConvertAttributeValueFromDynamic(input.RelatedEntityName);
                var relatedEntityAttribute = ConvertAttributeValueFromDynamic(input.RelatedEntityAttribute);

                ctx.AddRelationship(relationshipName, new XrmFakedRelationship
                {
                    IntersectEntity = intersectEntity,
                    Entity1LogicalName = entityName,
                    Entity1Attribute = entityAttribute,
                    Entity2LogicalName = relatedEntityName,
                    Entity2Attribute = relatedEntityAttribute
                });

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> AddNNRelationships(dynamic input)
        {
            try
            {
                var entityName = ConvertAttributeValueFromDynamic(input.EntityName);
                var entityId = ConvertAttributeValueFromDynamic(input.EntityId);
                var relationshipName = ConvertAttributeValueFromDynamic(input.RelationshipName);

                if (entityId is Guid)
                {
                    var relatedEntities = input.RelatedEntities as object[];

                    EntityReferenceCollection listOfEntities = new EntityReferenceCollection();
                    foreach (var entityRef in relatedEntities)
                    {
                        listOfEntities.Add((EntityReference)ConvertAttributeValueFromDynamic(entityRef));
                    }

                    var service = ctx.GetOrganizationService();

                    service.Associate(entityName, entityId, new Relationship(relationshipName), listOfEntities);
                }
                else
                    return "Cannot Cast EntityId to GUID."; 

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
