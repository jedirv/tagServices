from flask import jsonify

def get_simple_response(t, value):
    result = {}
    result["Type"] = t
    result["value"] = value
    response = jsonify(result)
    return response




def insert_rejected(cause):
    return get_simple_response("InsertRejected",cause)

def insert_succeeded(value):
    return get_simple_response("LastInsertId", value)


def delete_rejected(cause):
    return get_simple_response("DeleteRejected",cause)

def delete_succeeded(ID):
    return get_simple_response("DeleteId", ID)

def get_id_from_rows(all_rows):
    ID = '?'
    if (all_rows):
        row = all_rows[0]
        ID= row['ID']
    return ID