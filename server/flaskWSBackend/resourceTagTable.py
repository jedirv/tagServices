
import sqlitedb as db

def get_docs_for_tag(tagName):
    tagID = get_tag_ID_for_tagname(tagName)
    if (tagID=='?'):
	    return get_empty_doc_tree()
    else:
        return get_doc_tree_for_tag_id(tagID)

			
def get_doc_tree_for_tag_id(tagID):
    print("tag was {0}".format(tagID))
    mru_query = "SELECT Resources.Name FROM Resources INNER JOIN ResourceTags ON Resources.ID=ResourceTags.ResourceID WHERE Resources.Type='FILE' AND ResourceTags.TagID='{0}' ORDER BY Resources.LastUse DESC LIMIT 2;".format(tagID)
    mruDocList = []
    for doc_row in db.query_db(mru_query):
        doc = doc_row['Name']
        docDict = { "Name" : doc }
        mruDocList.append(docDict)
    
    rel_query = "SELECT Resources.Name FROM Resources INNER JOIN ResourceTags ON Resources.ID=ResourceTags.ResourceID WHERE Resources.Type='FILE' AND ResourceTags.TagID='{0}' ORDER BY Resources.Name;".format(tagID)
    relDocList = []
    for doc_row in db.query_db(rel_query):
        doc = doc_row['Name']
        docDict = { "Name" : doc }
        relDocList.append(docDict)
    result = {}
    result["RelevantDocuments"] = relDocList
    result["MruDocuments"] =  mruDocList
    response = jsonify(result);
    return response

    
def get_empty_doc_tree():
    mruDocList = []
    relDocList = []
    result = {}
    result["RelevantDocuments"] = relDocList
    result["MruDocuments"] =  mruDocList
    response = jsonify(result);
    return response
	
def get_dummy_document_tree():
	#'{\"RelevantDocuments\":[{\"Name\":\"docA1\"},{\"Name\":\"docA2\"},{\"Name\":\"docA3\"}],"MruDocuments":[{\"Name\":\"docA4\"},{\"Name\":\"docA5\"},{\"Name\":\"docA6\"}]}'
    doc1Dict = { "Name" : "docA1" }
    doc2Dict = { "Name" : "docA2" }
    doc3Dict = { "Name" : "docA3" }
    relDocList = []
    relDocList.append(doc1Dict);
    relDocList.append(doc2Dict);
    relDocList.append(doc3Dict);
    
    doc4Dict = { "Name" : "docA4" }
    doc5Dict = { "Name" : "docA5" }
    doc6Dict = { "Name" : "docB6" }
    mruDocList = []
    mruDocList.append(doc4Dict);
    mruDocList.append(doc5Dict);
    mruDocList.append(doc6Dict);
    
    result = {}
    result["RelevantDocuments"] = relDocList
    result["MruDocuments"] =  mruDocList
    response = jsonify(result);
    return response
	
	