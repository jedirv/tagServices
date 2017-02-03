
import sqlitedb as db
from flask import jsonify
import tagTable
import resourceTable
import util

def is_resource_tag_in_db(type, name, tag):
    print("resource is {0}, tag is {1}".format(name, tag))
    resourceTagID = get_resource_tag_id(type, name, tag)
    if (resourceTagID=='?'):
        return False
    return True

def get_resource_tag_id(type, name, tag):
    print("resource is {0}, tag is {1}".format(name, tag))
    resourceID = resourceTable.get_resource_id(type, name)
    tagID = tagTable.get_tag_id_for_tag(tag)
    if (resourceID=='?' or tagID=='?'):
        return '?'
    query="SELECT ResourceTags.ID FROM ResourceTags WHERE ResourceID='{0}' and TagID='{1}'".format(resourceID, tagID)
    
    all_rows = db.query_db(query)
    return util.get_id_from_rows(all_rows)

def add_resource_tag(type, name, tag):
    resourceID = resourceTable.get_resource_id(type, name)
    print('resourceID is {0}'.format(resourceID))
    tagID = tagTable.get_tag_id_for_tag(tag)
    if (tagID=='?'):
        return util.insert_rejected('tag {0} does not exist'.format(tag))
    if (resourceID=='?'):
        return util.insert_rejected('resource {0} {1} does not exist'.format(type, name))
    print('tagID is {0}'.format(tagID))
    query = "INSERT INTO ResourceTags (ResourceID, TagID) VALUES ('{0}','{1}');".format(resourceID, tagID)
    return db.insert_and_get_id(query, 'ResourceTags')


def get_docs_for_tag(tagName):
    tagID = tagTable.get_tag_id_for_tag(tagName)
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
	
	